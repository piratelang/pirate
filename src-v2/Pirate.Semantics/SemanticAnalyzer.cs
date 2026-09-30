using System.Collections.Generic;
using Pirate.Syntax;
using Pirate.Syntax.Nodes;

namespace Pirate.Semantics;

/// <summary>
/// Name resolution and static type checking per docs/GRAMMAR.md §2/§3:
/// two passes over a parsed program — first the top-level signatures
/// (externs and functions, so recursion is legal), then every function body
/// in its own scope. Errors are collected, never thrown; each expression is
/// checked once and annotated with its type so the compiler consumes an
/// already-checked AST.
///
/// Scopes: the global table holds functions and imported builtins; each
/// function gets one child table for parameters, locals and loop variables.
/// Blocks deliberately do not open scopes (matching the architecture doc's
/// Global/Local/Builtin/Free tiers); Free stays unused until closures.
/// </summary>
public sealed class SemanticAnalyzer : ISemanticAnalyzer
{
    private List<SemanticsError> _errors = new();
    private SymbolTable _global = null!;
    private SymbolTable _current = null!;
    private FunctionSymbol? _currentFunction;

    // Declarations whose signature pass failed (duplicate names): pass 2
    // skips them, so a duplicate reports one error instead of re-checking
    // the second body against the first declaration's symbol and cascading.
    private readonly HashSet<FunctionDeclarationNode> _failedSignatures = [];

    /// <inheritdoc/>
    /// <remarks>
    /// All mutable state is re-initialized per call, so one instance may be
    /// shared across analyses (the DI registration treats it as a singleton).
    /// </remarks>
    public SemanticResult Analyze(ProgramNode program)
    {
        _errors = [];
        _failedSignatures.Clear();
        Run(program);
        return new SemanticResult(program, _errors);
    }

    private void Run(ProgramNode program)
    {
        _global = new SymbolTable();
        _current = _global;

        foreach (var member in program.Members)
        {
            switch (member)
            {
                case ExternNode externNode:
                    DefineExtern(externNode);
                    break;
                case ImportStatementNode importNode:
                    DefineImport(importNode);
                    break;
                case FunctionDeclarationNode function:
                    DefineFunctionSignature(function);
                    break;
            }
        }

        // Top-level statements declare and mutate globals, so they are
        // checked before function bodies (which may read them). Only the
        // run entry-point module may contain them; enforcing that for
        // imported helper modules is part of the module-linking milestone —
        // for now every module's top-level code is type-checked as if run.
        CheckTopLevelStatements(program);

        foreach (var member in program.Members)
        {
            if (member is FunctionDeclarationNode function)
            {
                if (!_failedSignatures.Contains(function))
                {
                    CheckFunctionBody(function);
                }
            }
        }
    }

    // --- Pass 1: top-level definitions ---

    private void DefineImport(ImportStatementNode node)
    {
        var path = string.Join(".", node.Path);
        if (node.Kind is ImportKind.Module or ImportKind.External)
        {
            DefineModuleImport(node, path);
            return;
        }

        var namespaceRoot = "Standard." + path;
        var entries = BuiltinRegistry.InNamespace(namespaceRoot);
        if (entries.Count == 0)
        {
            Error(SemanticsErrorKind.UnknownImport, $"Unknown import 'standard {path}'", node.StartLocation);
            return;
        }

        // Build every binding the import would introduce (each builtin under
        // its dotted path — so imported names behave exactly like
        // extern-imported ones — and under its leaf name, the point of the
        // import form), check them all, and bind atomically: a collision
        // leaves the whole import unbound rather than half a namespace.
        var bindings = new List<BuiltinSymbol>(entries.Count * 2);
        foreach (var entry in entries)
        {
            var symbol = new BuiltinSymbol(entry.Name, entry.Index, SymbolScope.Builtin, entry.Parameters, entry.ReturnType);
            var leafName = entry.Name[(entry.Name.LastIndexOf('.') + 1)..];
            bindings.Add(symbol);
            if (leafName != entry.Name)
            {
                bindings.Add(symbol with { Name = leafName });
            }
        }

        foreach (var binding in bindings)
        {
            if (_global.Resolve(binding.Name) is not null)
            {
                Error(SemanticsErrorKind.DuplicateDeclaration, $"Duplicate declaration '{binding.Name}' in scope", node.StartLocation);
                return;
            }
        }

        node.ResolvedNamespace = namespaceRoot;
        foreach (var binding in bindings)
        {
            _global.Define(binding);
        }
    }

    /// <summary>
    /// Binds an <c>import module</c>/<c>import external</c> against the
    /// resolution the module linker recorded on the node
    /// (<see cref="ImportStatementNode.Resolution"/>): a resolved import
    /// binds its alias plus every export under the alias-qualified name, an
    /// unresolved one reports the linker's reason. A null resolution means
    /// no linker ran (a standalone single-module check, e.g. analyzing one
    /// program directly) — no project graph exists to resolve against.
    /// </summary>
    private void DefineModuleImport(ImportStatementNode node, string moduleName)
    {
        switch (node.Resolution)
        {
            case null:
                Error(
                    node.Kind == ImportKind.Module ? SemanticsErrorKind.UnknownModule : SemanticsErrorKind.UnknownDependency,
                    node.Kind == ImportKind.Module
                        ? $"Unknown module '{moduleName}'"
                        : $"Unknown external dependency '{moduleName}'",
                    node.StartLocation);
                return;
            case UnresolvedModuleImport unresolved:
                Error(UnresolvedKind(node.Kind, unresolved.Reason), UnresolvedMessage(node.Kind, moduleName, unresolved), node.StartLocation);
                return;
            case ResolvedModuleImport resolved:
                BindModuleExports(node, moduleName, resolved);
                return;
        }
    }

    private void BindModuleExports(ImportStatementNode node, string moduleName, ResolvedModuleImport resolved)
    {
        var alias = node.Alias ?? node.Path[^1];

        var bindings = new List<Symbol> { new ModuleSymbol(alias, _global.Count, SymbolScope.Module, moduleName) };
        foreach (var export in resolved.Exports)
        {
            // Exports are the providing module's own declarations; a name it
            // imported itself is never re-exported (defensive: the grammar
            // only allows public-by-default/private on function/variable
            // declarations, not on an imported alias).
            switch (export)
            {
                case ImportedVariableSymbol or ImportedFunctionSymbol:
                    break;
                case VariableSymbol variable:
                    bindings.Add(new ImportedVariableSymbol($"{alias}.{variable.Name}", variable.Index, variable.Type, variable.IsConst, moduleName));
                    break;
                case FunctionSymbol function:
                    bindings.Add(new ImportedFunctionSymbol($"{alias}.{function.Name}", function.Index, function.Parameters, function.ReturnType, moduleName));
                    break;
            }
        }

        // Bind all-or-nothing, the same atomicity policy as standard
        // imports: any collision rejects the whole import, leaving the
        // alias unbound rather than half an interface reachable.
        foreach (var binding in bindings)
        {
            if (_global.Resolve(binding.Name) is not null)
            {
                Error(SemanticsErrorKind.DuplicateDeclaration, $"Duplicate declaration '{binding.Name}' in scope", node.StartLocation);
                return;
            }
        }

        foreach (var binding in bindings)
        {
            _global.Define(binding);
        }
    }

    private static SemanticsErrorKind UnresolvedKind(ImportKind importKind, UnresolvedModuleReason reason) => reason switch
    {
        UnresolvedModuleReason.NotFound => importKind == ImportKind.Module
            ? SemanticsErrorKind.UnknownModule
            : SemanticsErrorKind.UnknownDependency,
        UnresolvedModuleReason.LocationMissing => SemanticsErrorKind.DependencyLocationMissing,
        UnresolvedModuleReason.RemoteLocation => SemanticsErrorKind.RemoteDependencyUnsupported,
        UnresolvedModuleReason.CyclicImport => SemanticsErrorKind.CyclicModuleImport,
        UnresolvedModuleReason.HasTopLevelStatements => SemanticsErrorKind.ImportedModuleHasTopLevelCode,
        UnresolvedModuleReason.HasErrors => SemanticsErrorKind.ImportedModuleHasErrors,
        _ => SemanticsErrorKind.UnknownModule,
    };

    private static string UnresolvedMessage(ImportKind importKind, string moduleName, UnresolvedModuleImport unresolved) =>
        unresolved.Reason switch
        {
            UnresolvedModuleReason.NotFound => importKind == ImportKind.Module
                ? $"Unknown module '{moduleName}'"
                : $"Unknown external dependency '{moduleName}'",
            UnresolvedModuleReason.LocationMissing =>
                $"External dependency '{moduleName}' points to '{unresolved.Location}', which does not exist",
            UnresolvedModuleReason.RemoteLocation =>
                $"External dependency '{moduleName}' has a remote location ('{unresolved.Location}') — fetching remote modules is not implemented yet",
            UnresolvedModuleReason.CyclicImport => unresolved.CyclePath is { Count: > 1 } cyclePath
                ? $"Cyclic module import: {string.Join(" → ", cyclePath.Select(name => $"'{name}'"))}"
                : $"Cyclic module import involving '{moduleName}'",
            UnresolvedModuleReason.HasTopLevelStatements =>
                $"Module '{moduleName}' has top-level statements and cannot be imported — only the entry module runs code",
            UnresolvedModuleReason.HasErrors =>
                $"Module '{moduleName}' has compilation errors; its exports are unavailable",
            _ => $"Unknown module '{moduleName}'",
        };

    private void CheckTopLevelStatements(ProgramNode program)
    {
        _current = _global;
        _currentFunction = null;
        foreach (var statement in program.Statements)
        {
            CheckStatement(statement);
        }
    }

    private void DefineExtern(ExternNode node)
    {
        var path = string.Join(".", node.QualifiedName);
        var entry = BuiltinRegistry.Lookup(path);
        if (entry is null)
        {
            Error(SemanticsErrorKind.UnknownExtern, $"Unknown extern '{path}'", node.StartLocation);
            return;
        }

        if (_global.Define(new BuiltinSymbol(path, entry.Index, SymbolScope.Builtin, entry.Parameters, entry.ReturnType)) is not null)
        {
            Error(SemanticsErrorKind.DuplicateDeclaration, $"Duplicate declaration '{path}' in scope", node.StartLocation);
        }
    }

    private void DefineFunctionSignature(FunctionDeclarationNode node)
    {
        var parameterTypes = new List<PirateType>(node.Parameters.Count);
        foreach (var parameter in node.Parameters)
        {
            // A failed conversion is already reported; Void keeps the
            // parameter list aligned with the body's locals so the second
            // pass doesn't re-report the same arity.
            parameterTypes.Add(ToValueType(parameter.Type) ?? PirateType.Void);
        }

        var returnType = ToReturnType(node);

        if (_global.Resolve(node.Name) is not null)
        {
            Error(SemanticsErrorKind.DuplicateDeclaration, $"Duplicate declaration '{node.Name}' in scope", node.StartLocation);
            _failedSignatures.Add(node);
            return;
        }

        _global.Define(new FunctionSymbol(
            node.Name,
            _global.Count,
            SymbolScope.Global,
            parameterTypes,
            returnType));
    }

    // --- Pass 2: function bodies ---

    private void CheckFunctionBody(FunctionDeclarationNode node)
    {
        var symbol = _global.Resolve(node.Name) as FunctionSymbol;
        _current = new SymbolTable(_global);
        _currentFunction = symbol;

        if (symbol is not null)
        {
            for (var i = 0; i < node.Parameters.Count; i++)
            {
                var parameter = node.Parameters[i];
                var type = i < symbol.Parameters.Count ? symbol.Parameters[i] : PirateType.Void;
                var variable = new VariableSymbol(parameter.Name, _current.Count, SymbolScope.Local, type, IsConst: false);
                if (_current.Define(variable) is not null)
                {
                    // Duplicate parameter names collide here, in function scope.
                    Error(SemanticsErrorKind.DuplicateDeclaration, $"Duplicate declaration '{parameter.Name}' in scope", parameter.StartLocation);
                    continue;
                }

                parameter.ResolvedSymbol = variable;
            }
        }

        CheckBlock(node.Body);

        if (symbol is not null && symbol.ReturnType != PirateType.Void && !BlockReturnsOnAllPaths(node.Body))
        {
            Error(SemanticsErrorKind.MissingReturnInNonVoidFunction, $"Function '{node.Name}' has no reachable 'return'", node.StartLocation);
        }

        _current = _global;
        _currentFunction = null;
    }

    private void CheckBlock(BlockNode block)
    {
        foreach (var statement in block.Statements)
        {
            CheckStatement(statement);
        }

        if (block.ReturnStatement is not null)
        {
            CheckReturn(block.ReturnStatement);
        }
    }

    private void CheckStatement(StatementNode statement)
    {
        switch (statement)
        {
            case VariableDeclarationNode declaration:
                CheckVariableDeclaration(declaration);
                break;
            case VariableAssignmentNode assignment:
                CheckVariableAssignment(assignment);
                break;
            case MemberAssignmentNode memberAssignment:
                CheckMemberAssignment(memberAssignment);
                break;
            case ExpressionStatementNode expressionStatement:
                CheckExpressionStatement(expressionStatement);
                break;
            case IfStatementNode ifStatement:
                CheckIf(ifStatement);
                break;
            case WhileStatementNode whileStatement:
                CheckWhile(whileStatement);
                break;
            case ForStatementNode forStatement:
                CheckFor(forStatement);
                break;
            case ForInStatementNode forInStatement:
                CheckForIn(forInStatement);
                break;
            case ReturnStatementNode returnStatement:
                CheckReturn(returnStatement);
                break;
        }
    }

    // --- Statements ---

    private void CheckVariableDeclaration(VariableDeclarationNode node)
    {
        PirateType? type;
        if (node.Type is not null)
        {
            type = ToValueType(node.Type);
            if (type is null)
            {
                return;
            }

            ExpectType(node.Initializer, type.Value);
        }
        else
        {
            type = CheckExpression(node.Initializer, null);
            if (type is null)
            {
                return;
            }

            if (type.Value == PirateType.Void)
            {
                Error(SemanticsErrorKind.TypeMismatch, $"Cannot infer type of '{node.Name}': initializer has type 'void'", node.StartLocation);
                return;
            }
        }

        var scope = _current == _global ? SymbolScope.Global : SymbolScope.Local;
        var symbol = new VariableSymbol(node.Name, _current.Count, scope, type.Value, node.IsConst);
        if (_current.Define(symbol) is not null)
        {
            Error(SemanticsErrorKind.DuplicateDeclaration, $"Duplicate declaration '{node.Name}' in scope", node.StartLocation);
            return;
        }

        node.ResolvedSymbol = symbol;
    }

    private void CheckVariableAssignment(VariableAssignmentNode node)
    {
        var symbol = _current.Resolve(node.Name);
        if (symbol is null)
        {
            Error(SemanticsErrorKind.UndeclaredVariable, $"Undeclared variable '{node.Name}'", node.StartLocation);
            CheckExpression(node.Index, null);
            CheckExpression(node.Value, null);
            return;
        }

        if (symbol is not VariableSymbol variable)
        {
            Error(SemanticsErrorKind.TypeMismatch, $"'{node.Name}' is a function and cannot be reassigned", node.StartLocation);
            return;
        }

        if (variable.IsConst)
        {
            Error(SemanticsErrorKind.AssignmentToConst, $"Cannot assign to '{node.Name}' (declared 'const')", node.StartLocation);
            return;
        }

        if (node.Index is not null)
        {
            if (!variable.Type.IsArray)
            {
                Error(SemanticsErrorKind.TypeMismatch, $"Cannot index '{node.Name}' of non-array type '{variable.Type}'", node.StartLocation);
                return;
            }

            ExpectType(node.Index, PirateType.Int);
            ExpectType(node.Value, variable.Type.ElementType!.Value);
            return;
        }

        ExpectType(node.Value, variable.Type);
    }

    /// <summary>
    /// <c>self.count = 1;</c>, <c>c.count = 1;</c> (docs/GRAMMAR.md §4.3).
    /// Nothing resolves a member to an assignable field yet, so every
    /// instance of this node is rejected — classes are a future milestone
    /// (Phase 4 of docs/brainstorm/FLAT_PLAN.md). Subexpressions are still
    /// checked so an error inside them isn't silently skipped.
    /// </summary>
    private void CheckMemberAssignment(MemberAssignmentNode node)
    {
        CheckExpression(node.Target, null);
        CheckExpression(node.Value, null);
        Error(SemanticsErrorKind.TypeMismatch, "Member assignment is not supported yet — classes are a future milestone", node.StartLocation);
    }

    private void CheckExpressionStatement(ExpressionStatementNode node)
    {
        if (node.Expression is FunctionCallNode call)
        {
            CheckExpression(call, null);
            return;
        }

        CheckExpression(node.Expression, null);
        Error(SemanticsErrorKind.TypeMismatch, "Expression statement must be a function call", node.StartLocation);
    }

    private void CheckIf(IfStatementNode node)
    {
        var condition = CheckExpression(node.Condition, null);
        if (condition is not null && condition.Value != PirateType.Bool)
        {
            Error(SemanticsErrorKind.TypeMismatch, $"'if' condition must be 'bool', got '{condition}'", node.Condition.StartLocation);
        }

        CheckBlock(node.ThenBranch);

        switch (node.ElseBranch)
        {
            case BlockNode block:
                CheckBlock(block);
                break;
            case IfStatementNode nested:
                CheckIf(nested);
                break;
        }
    }

    private void CheckWhile(WhileStatementNode node)
    {
        var condition = CheckExpression(node.Condition, null);
        if (condition is not null && condition.Value != PirateType.Bool)
        {
            Error(SemanticsErrorKind.TypeMismatch, $"'while' condition must be 'bool', got '{condition}'", node.Condition.StartLocation);
        }

        CheckBlock(node.Body);
    }

    private void CheckFor(ForStatementNode node)
    {
        ExpectType(node.Start, PirateType.Int);
        ExpectType(node.End, PirateType.Int);
        node.LoopVariable = DefineLoopVariable(node.VariableName, node.StartLocation, PirateType.Int);
        CheckBlock(node.Body);
    }

    private void CheckForIn(ForInStatementNode node)
    {
        var iterable = CheckExpression(node.Iterable, null);
        if (iterable is null)
        {
            return;
        }

        if (!iterable.Value.IsArray)
        {
            Error(SemanticsErrorKind.ForInIterableMustBeArray, $"for-in iterable must be an array, got '{iterable}'", node.Iterable.StartLocation);
            return;
        }

        node.LoopVariable = DefineLoopVariable(node.VariableName, node.StartLocation, iterable.Value.ElementType!.Value);
        CheckBlock(node.Body);
    }

    private VariableSymbol? DefineLoopVariable(string name, SourceLocation at, PirateType type)
    {
        var scope = _current == _global ? SymbolScope.Global : SymbolScope.Local;
        var symbol = new VariableSymbol(name, _current.Count, scope, type, IsConst: false);
        if (_current.Define(symbol) is not null)
        {
            Error(SemanticsErrorKind.DuplicateDeclaration, $"Duplicate declaration '{name}' in scope", at);
            return null;
        }

        return symbol;
    }

    private void CheckReturn(ReturnStatementNode node)
    {
        var returnType = _currentFunction?.ReturnType;
        if (returnType is null)
        {
            // Only reachable outside a function body: a 'return' sitting in
            // a module's top-level statements.
            Error(SemanticsErrorKind.ReturnAtTopLevel, "'return' is only valid inside a function", node.StartLocation);
            return;
        }

        if (node.Value is null)
        {
            if (returnType.Value != PirateType.Void)
            {
                Error(SemanticsErrorKind.ReturnValueRequired, "Return value required in non-void function", node.StartLocation);
            }

            return;
        }

        if (returnType.Value == PirateType.Void)
        {
            Error(SemanticsErrorKind.VoidFunctionReturnsValue, "Void function cannot return a value", node.StartLocation);
            return;
        }

        ExpectType(node.Value, returnType.Value);
    }

    /// <summary>
    /// A block returns on all paths if it ends in a return statement, or its
    /// last statement is an if/else whose every branch returns on all paths.
    /// Per grammar §3.3 a return can only ever be a block's final statement,
    /// so this structural check is exactly the path coverage the language can
    /// express.
    /// </summary>
    private static bool BlockReturnsOnAllPaths(BlockNode block)
    {
        if (block.ReturnStatement is not null)
        {
            return true;
        }

        if (block.Statements.Count > 0 && block.Statements[^1] is IfStatementNode ifStatement && AllPathsCover(ifStatement))
        {
            return true;
        }

        return false;
    }

    private static bool AllPathsCover(IfStatementNode node) =>
        node.ElseBranch is not null &&
        BranchReturnsOnAllPaths(node.ThenBranch) &&
        BranchReturnsOnAllPaths(node.ElseBranch);

    private static bool BranchReturnsOnAllPaths(StatementNode branch) => branch switch
    {
        BlockNode block => BlockReturnsOnAllPaths(block),
        IfStatementNode nested => AllPathsCover(nested),
        _ => false,
    };

    // --- Expressions ---

    /// <summary>
    /// Computes and annotates the type of an expression; null means an error
    /// was reported (callers must not cascade a second error for the same
    /// subexpression). <paramref name="expected"/> only guides cases the
    /// expression cannot determine alone — currently the empty array literal,
    /// which needs its element type from the declaration context.
    /// </summary>
    private PirateType? CheckExpression(ExpressionNode? expression, PirateType? expected)
    {
        if (expression is null)
        {
            return null;
        }

        var type = expression switch
        {
            LiteralNode literal => CheckLiteral(literal),
            QualifiedNameNode qualifiedName => CheckQualifiedName(qualifiedName),
            MemberAccessNode memberAccess => CheckMemberAccess(memberAccess),
            FunctionCallNode call => CheckFunctionCall(call),
            BinaryOperationNode binary => CheckBinary(binary),
            UnaryOperationNode unary => CheckUnary(unary),
            ArrayLiteralNode array => CheckArrayLiteral(array, expected),
            IndexExpressionNode index => CheckIndex(index),
            _ => null,
        };

        if (type is not null)
        {
            expression.InferredType = type;
        }

        return type;
    }

    private PirateType CheckLiteral(LiteralNode node) => node.LiteralKind switch
    {
        LiteralKind.Int => PirateType.Int,
        LiteralKind.Float => PirateType.Float,
        LiteralKind.String => PirateType.String,
        LiteralKind.Char => PirateType.Char,
        LiteralKind.Bool => PirateType.Bool,
        _ => PirateType.Bool,
    };

    private PirateType? CheckQualifiedName(QualifiedNameNode node)
    {
        var symbol = _current.Resolve(node.Name);
        if (symbol is null)
        {
            Error(SemanticsErrorKind.UndeclaredVariable, $"Undeclared variable '{node.Name}'", node.StartLocation);
            return null;
        }

        node.ResolvedSymbol = symbol;
        if (symbol is VariableSymbol variable)
        {
            return variable.Type;
        }

        Error(SemanticsErrorKind.TypeMismatch, $"'{node.Name}' is a function and cannot be used as a value", node.StartLocation);
        return null;
    }

    /// <summary>
    /// A member access used as a value, e.g. <c>Standard.Terminal.Read()</c>'s
    /// <c>Standard.Terminal</c> would hit this if it weren't inside a call —
    /// reachable directly for a dotted name with no call, like
    /// <c>var f = Standard.Terminal.Print;</c> (rejected below, same as a
    /// bare function name). A non-dotted-name target (docs/GRAMMAR.md §4.4
    /// namespace-prefix resolution doesn't apply) is a future-milestone
    /// object field access — not supported until classes exist (Phase 4 of
    /// docs/brainstorm/FLAT_PLAN.md).
    /// </summary>
    private PirateType? CheckMemberAccess(MemberAccessNode node)
    {
        var path = FlattenDottedPath(node);
        if (path is null)
        {
            CheckExpression(node.Target, null);
            Error(SemanticsErrorKind.TypeMismatch, "Member access is not supported yet — classes are a future milestone", node.StartLocation);
            return null;
        }

        var symbol = _current.Resolve(path);
        if (symbol is null)
        {
            Error(SemanticsErrorKind.UndeclaredFunction, $"Undeclared function '{path}'", node.StartLocation);
            return null;
        }

        node.ResolvedSymbol = symbol;
        if (symbol is VariableSymbol variable)
        {
            return variable.Type;
        }

        Error(SemanticsErrorKind.TypeMismatch, $"'{path}' is a function and cannot be used as a value", node.StartLocation);
        return null;
    }

    /// <summary>
    /// Reconstructs the dotted path a chain of bare names/member accesses
    /// spells (<c>Standard.Terminal.Print</c>) — the resolver still keys
    /// builtins/imports by this joined string (docs/GRAMMAR.md §4.4's
    /// "longest namespace prefix" resolution), only the AST shape that
    /// produces it changed (§3.6's member-suffix postfix, replacing the old
    /// flat <c>QualifiedNameNode.Parts</c> chain). Null when the chain's
    /// innermost expression isn't a bare name — e.g. <c>f().b</c> — so
    /// there is no namespace path to look up.
    /// </summary>
    private static string? FlattenDottedPath(ExpressionNode node) => node switch
    {
        QualifiedNameNode name => name.Name,
        MemberAccessNode access => FlattenDottedPath(access.Target) is { } prefix ? $"{prefix}.{access.Member}" : null,
        _ => null,
    };

    /// <summary>Sets the resolved symbol on whichever name-shaped node produced a dotted path (see <see cref="FlattenDottedPath"/>).</summary>
    private static void SetResolvedSymbol(ExpressionNode node, Symbol symbol)
    {
        switch (node)
        {
            case QualifiedNameNode name:
                name.ResolvedSymbol = symbol;
                break;
            case MemberAccessNode access:
                access.ResolvedSymbol = symbol;
                break;
        }
    }

    private PirateType? CheckFunctionCall(FunctionCallNode node)
    {
        var path = FlattenDottedPath(node.Callee);
        if (path is null)
        {
            CheckExpression(node.Callee, null);
            foreach (var argument in node.Arguments)
            {
                CheckExpression(argument, null);
            }

            Error(SemanticsErrorKind.TypeMismatch, "Only declared functions can be called", node.StartLocation);
            return null;
        }

        var symbol = _current.Resolve(path);
        if (symbol is null)
        {
            Error(SemanticsErrorKind.UndeclaredFunction, $"Undeclared function '{path}'", node.Callee.StartLocation);
            foreach (var argument in node.Arguments)
            {
                CheckExpression(argument, null);
            }

            return null;
        }

        SetResolvedSymbol(node.Callee, symbol);
        if (symbol is VariableSymbol)
        {
            foreach (var argument in node.Arguments)
            {
                CheckExpression(argument, null);
            }

            Error(SemanticsErrorKind.TypeMismatch, $"'{path}' is not a function", node.StartLocation);
            return null;
        }

        IReadOnlyList<PirateType> parameters = symbol switch
        {
            FunctionSymbol function => function.Parameters,
            BuiltinSymbol builtin => builtin.Parameters,
            _ => [],
        };

        if (node.Arguments.Count != parameters.Count)
        {
            Error(
                SemanticsErrorKind.TypeMismatch,
                $"Wrong number of arguments to '{path}': expected {parameters.Count}, got {node.Arguments.Count}",
                node.StartLocation);
        }

        for (var i = 0; i < node.Arguments.Count; i++)
        {
            if (i < parameters.Count)
            {
                ExpectType(node.Arguments[i], parameters[i]);
            }
            else
            {
                CheckExpression(node.Arguments[i], null);
            }
        }

        node.ResolvedCallee = symbol;
        return symbol switch
        {
            FunctionSymbol function => function.ReturnType,
            BuiltinSymbol builtin => builtin.ReturnType,
            _ => PirateType.Void,
        };
    }

    private PirateType? CheckBinary(BinaryOperationNode node)
    {
        var left = CheckExpression(node.Left, null);
        var right = CheckExpression(node.Right, null);
        if (left is null || right is null)
        {
            return null;
        }

        PirateType? result = node.Operator switch
        {
            // Thorsten Ball default: '+' adds integers and concatenates strings.
            BinaryOperator.Add when left.Value.IsNumeric && right.Value.IsNumeric && left.Value == right.Value => left,
            BinaryOperator.Add when left == PirateType.String && right == PirateType.String => left,
            BinaryOperator.Subtract or BinaryOperator.Multiply or BinaryOperator.Divide
                when left.Value.IsNumeric && right.Value.IsNumeric && left.Value == right.Value => left,
            // '%' and '^' have no Monkey equivalent (it has one integer type and
            // no power operator); int-only is the conservative default, and the
            // wider numeric domains are follow-up issues.
            BinaryOperator.Modulo or BinaryOperator.Power
                when left == PirateType.Int && right == PirateType.Int => left,
            BinaryOperator.Equal or BinaryOperator.NotEqual
                when left == right && !left.Value.IsArray => PirateType.Bool,
            BinaryOperator.Less or BinaryOperator.LessEqual or BinaryOperator.Greater or BinaryOperator.GreaterEqual
                when left.Value.IsNumeric && right.Value.IsNumeric && left.Value == right.Value => PirateType.Bool,
            BinaryOperator.And or BinaryOperator.Or
                when left == PirateType.Bool && right == PirateType.Bool => PirateType.Bool,
            _ => null,
        };

        if (result is null)
        {
            var isUnsupportedArrayEquality = node.Operator is BinaryOperator.Equal or BinaryOperator.NotEqual && left == right;
            var message = isUnsupportedArrayEquality
                ? $"Operator '{OperatorName(node.Operator)}' cannot be applied to array type '{left}'"
                : $"Operator '{OperatorName(node.Operator)}' cannot be applied to '{left}' and '{right}'";
            Error(SemanticsErrorKind.TypeMismatch, message, node.StartLocation);
            return null;
        }

        return result;
    }

    private PirateType? CheckUnary(UnaryOperationNode node)
    {
        var operand = CheckExpression(node.Operand, null);
        if (operand is null)
        {
            return null;
        }

        PirateType? result = node.Operator switch
        {
            UnaryOperator.Negate when operand.Value.IsNumeric => operand,
            UnaryOperator.Not when operand == PirateType.Bool => operand,
            _ => null,
        };

        if (result is null)
        {
            var symbol = node.Operator == UnaryOperator.Not ? "!" : "-";
            Error(SemanticsErrorKind.TypeMismatch, $"Operator '{symbol}' cannot be applied to '{operand}'", node.StartLocation);
            return null;
        }

        return result;
    }

    private PirateType? CheckArrayLiteral(ArrayLiteralNode node, PirateType? expected)
    {
        if (node.Elements.Count == 0)
        {
            if (expected is { IsArray: true })
            {
                return expected;
            }

            Error(SemanticsErrorKind.EmptyArrayRequiresElementType, "Empty array literal has no element type to infer", node.StartLocation);
            return null;
        }

        if (expected is { IsArray: true } expectedArray)
        {
            foreach (var element in node.Elements)
            {
                ExpectType(element, expectedArray.ElementType!.Value);
            }

            return expected;
        }

        var first = CheckExpression(node.Elements[0], null);
        if (first is null)
        {
            return null;
        }

        if (first.Value.IsArray)
        {
            Error(SemanticsErrorKind.TypeMismatch, "Nested arrays are not supported", node.StartLocation);
            return null;
        }

        foreach (var element in node.Elements.Skip(1))
        {
            var type = CheckExpression(element, null);
            if (type is not null && type.Value != first.Value)
            {
                Error(SemanticsErrorKind.TypeMismatch, $"Array element type mismatch: expected '{first}', got '{type}'", element.StartLocation);
            }
        }

        return PirateType.ArrayOf(first.Value.Scalar);
    }

    private PirateType? CheckIndex(IndexExpressionNode node)
    {
        var target = CheckExpression(node.Target, null);
        if (target is null)
        {
            CheckExpression(node.Index, null);
            return null;
        }

        if (!target.Value.IsArray)
        {
            CheckExpression(node.Index, null);
            Error(SemanticsErrorKind.TypeMismatch, $"Cannot index value of non-array type '{target}'", node.StartLocation);
            return null;
        }

        ExpectType(node.Index, PirateType.Int);
        return target.Value.ElementType;
    }

    // --- Helpers ---

    private PirateType? ExpectType(ExpressionNode expression, PirateType expected)
    {
        var actual = CheckExpression(expression, expected);
        if (actual is null)
        {
            return null;
        }

        if (actual.Value != expected)
        {
            Error(SemanticsErrorKind.TypeMismatch, $"Type mismatch: expected '{expected}', got '{actual}'", expression.StartLocation);
            return null;
        }

        return actual;
    }

    /// <summary>
    /// Converts a syntax <c>TypeNode</c> to a value type: <c>void</c> is not a
    /// value type (GRAMMAR.md §2), so it fails here unless this is a function
    /// return position.
    /// </summary>
    private PirateType? ToValueType(TypeNode node)
    {
        if (node.ScalarType == ScalarType.Void)
        {
            Error(SemanticsErrorKind.TypeMismatch, "'void' is not a value type", node.StartLocation);
            return null;
        }

        return ToType(node);
    }

    /// <summary>
    /// Carries every field of a syntax <c>TypeNode</c> into the value type,
    /// so a future kind of type (a class name, or <c>IsNullable</c> once the
    /// parser produces one) doesn't need this conversion edited again —
    /// callers that need to reject a particular shape (like <c>void</c> in
    /// <see cref="ToValueType"/>) check the result themselves.
    /// </summary>
    private static PirateType ToType(TypeNode node) =>
        new(node.ScalarType, node.IsArray, node.IsNullable, node.ClassName);

    private PirateType ToReturnType(FunctionDeclarationNode node)
    {
        if (node.ReturnType.ScalarType == ScalarType.Void)
        {
            if (node.ReturnType.IsArray)
            {
                Error(SemanticsErrorKind.TypeMismatch, "'void' is not a value type, so 'void[]' is not a type", node.ReturnType.StartLocation);
                return PirateType.Void;
            }

            return PirateType.Void;
        }

        return ToType(node.ReturnType);
    }

    private static string OperatorName(BinaryOperator operatorName) => operatorName switch
    {
        BinaryOperator.Add => "+",
        BinaryOperator.Subtract => "-",
        BinaryOperator.Multiply => "*",
        BinaryOperator.Divide => "/",
        BinaryOperator.Modulo => "%",
        BinaryOperator.Power => "^",
        BinaryOperator.Equal => "==",
        BinaryOperator.NotEqual => "!=",
        BinaryOperator.Less => "<",
        BinaryOperator.LessEqual => "<=",
        BinaryOperator.Greater => ">",
        BinaryOperator.GreaterEqual => ">=",
        BinaryOperator.And => "&&",
        BinaryOperator.Or => "||",
        _ => operatorName.ToString(),
    };

    private void Error(SemanticsErrorKind kind, string message, SourceLocation at) =>
        _errors.Add(new SemanticsError(kind, message, at));
}
