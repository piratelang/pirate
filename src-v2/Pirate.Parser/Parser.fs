namespace Pirate.Parser

open System.Collections.Generic
open Pirate.Lexer
open Pirate.Syntax
open Pirate.Syntax.Nodes

/// <summary>
/// Top-level and statement parser that delegates to <see cref="Pratt"/> for
/// expressions. Recursive-descent for statements, Pratt for expressions.
/// Per docs/GRAMMAR.md §3.
/// </summary>
module Parser =

    let private eof state = state.Pos >= state.Tokens.Count - 1
    let private peek state =
        if state.Pos >= state.Tokens.Count then state.Tokens.[state.Tokens.Count - 1]
        else state.Tokens.[state.Pos]
    // Advances carefully: never moves past the final Eof token, so a follow-up
    // peek is always in range even after an error path consumed the Eof token.
    let private advance state =
        let t = state.Tokens.[state.Pos]
        if state.Pos < state.Tokens.Count - 1 then state.Pos <- state.Pos + 1
        t
    let private at state t = not (eof state) && peek state |> fun tok -> tok.Type = t
    let private loc (t: Token) = t.Location

    let private expect state tokenType kind msg =
        if at state tokenType then
            Some (advance state)
        else
            state.Errors.Add(SyntaxError(kind, msg, loc (peek state)))
            None

    // --- Type ---
    // type = base-type [ '?' ] [ '[' ']' [ '?' ] ] ; base-type = scalar-type | qualified-name ;
    // A bare identifier is a class-name base type (docs/GRAMMAR.md §4) —
    // resolving it against the project's types is Phase 3/4 of
    // docs/brainstorm/FLAT_PLAN.md, so the parser accepts any name here.
    let private parseType state : TypeNode option =
        let startTok = peek state
        let baseType =
            match startTok.Type with
            | TokenType.Int        -> Some (Some ScalarType.Int, None)
            | TokenType.Float      -> Some (Some ScalarType.Float, None)
            | TokenType.String     -> Some (Some ScalarType.String, None)
            | TokenType.Char       -> Some (Some ScalarType.Char, None)
            | TokenType.Bool       -> Some (Some ScalarType.Bool, None)
            | TokenType.Void       -> Some (Some ScalarType.Void, None)
            | TokenType.Identifier -> Some (None, Some startTok.Lexeme)
            | _ ->
                state.Errors.Add(SyntaxError(SyntaxErrorKind.ExpectedType, sprintf "Expected type, got '%s'" startTok.Lexeme, loc startTok))
                None
        match baseType with
        | None -> None
        | Some (scalarOpt, classNameOpt) ->
            advance state |> ignore // consume the scalar keyword or class-name identifier
            // T?[]/T[]? both set one flag — distinguishing nullable elements
            // from a nullable array is deferred (see PirateType.IsNullable).
            let mutable isNullable = false
            if at state TokenType.Question then
                advance state |> ignore
                isNullable <- true
            let mutable isArr = false
            if at state TokenType.LeftBracket then
                advance state |> ignore
                if at state TokenType.RightBracket then
                    advance state |> ignore
                    isArr <- true
                    if at state TokenType.Question then
                        advance state |> ignore
                        isNullable <- true
                else
                    state.Errors.Add(SyntaxError(SyntaxErrorKind.MissingCloseBracketInType, "Expected ']' after '[' in type", loc (peek state)))
            // End at the last consumed token, not the next token.
            let endLoc = loc (state.Tokens.[state.Pos - 1])
            // Placeholder when this is a class type — ClassName is what matters then.
            let scalar = scalarOpt |> Option.defaultValue ScalarType.Int
            Some (TypeNode(loc startTok, endLoc, scalar, isArr, isNullable, classNameOpt |> Option.toObj))

    // --- Block ---
    let rec private parseBlock state : BlockNode option =
        match expect state TokenType.LeftBrace SyntaxErrorKind.MissingOpenBrace "Expected '{' to begin block" with
        | None -> None
        | Some openBrace ->
            let stmts = ResizeArray<StatementNode>()
            let mutable retNode: ReturnStatementNode = Unchecked.defaultof<_>
            while not (eof state) && not (at state TokenType.RightBrace) do
                match parseStatement state with
                | Some (:? ReturnStatementNode as rs) -> retNode <- rs
                | Some s -> stmts.Add(s)
                | None -> advance state |> ignore
            let closeBrace =
                match expect state TokenType.RightBrace SyntaxErrorKind.MissingCloseBrace "Expected '}' to close block" with
                | Some t -> t
                | None -> openBrace
            // C# nullable BlockNode.ReturnStatement: retNode stays null when no return was seen.
            Some (BlockNode(loc openBrace, loc closeBrace, stmts, retNode))

    // --- Statements ---
    and private parseStatement state : StatementNode option =
        if eof state || at state TokenType.RightBrace then
            None
        else
            match (peek state).Type with
            | TokenType.Return -> parseReturn state
            | TokenType.If     -> parseIf state
            | TokenType.While  -> parseWhile state
            | TokenType.For    -> parseFor state
            | TokenType.Var    -> parseVarDecl state (peek state) false false
            | TokenType.Const  -> parseConstDecl state (peek state) false
            | TokenType.Int
            | TokenType.Float
            | TokenType.String
            | TokenType.Char
            | TokenType.Bool   -> parseTypedVarDecl state (peek state) false false
            | _                -> parseExprStmt state

    // variable-declaration = [ 'const' ] [ ( type | 'var' ) ] identifier '=' expression ';'
    // A 'const' with no type or 'var' infers its type from the initializer;
    // plain (non-const) declarations still require type or 'var' so that
    // 'x = 5;' stays an assignment, not a re-inferable declaration.
    and private parseConstDecl state startToken isPrivate : StatementNode option =
        advance state |> ignore // consume 'const'; startToken anchors the node
        match (peek state).Type with
        | TokenType.Var -> parseVarDecl state startToken true isPrivate
        | TokenType.Int
        | TokenType.Float
        | TokenType.String
        | TokenType.Char
        | TokenType.Bool -> parseTypedVarDecl state startToken true isPrivate
        | _ -> parseDeclarationTail state startToken true isPrivate
                 SyntaxErrorKind.MissingIdentifierAfterConst "Expected identifier after 'const'"

    and private parseReturn state : StatementNode option =
        let startTok = advance state
        // C# nullable ReturnStatementNode.Value: stays null when the return has no value.
        let mutable valueNode: ExpressionNode = Unchecked.defaultof<_>
        if not (at state TokenType.Semicolon) then
            valueNode <- Pratt.parseExpression state 0
        expect state TokenType.Semicolon SyntaxErrorKind.MissingSemicolonAfterReturn "Expected ';' after return" |> ignore
        Some (ReturnStatementNode(loc startTok, loc (peek state), valueNode) :> StatementNode)

    and private parseIf state : StatementNode option =
        let startTok = advance state
        let cond = Pratt.parseExpression state 0
        match parseBlock state with
        | None -> None
        | Some thenBlock ->
            // C# nullable IfStatementNode.ElseBranch: null when there is no else.
            let mutable elseBranch: StatementNode = Unchecked.defaultof<_>
            if at state TokenType.Else then
                advance state |> ignore
                if at state TokenType.If then
                    match parseIf state with
                    | Some nested -> elseBranch <- nested
                    | None -> ()
                else
                    match parseBlock state with
                    | Some b -> elseBranch <- b :> StatementNode
                    | None -> ()
            Some (IfStatementNode(loc startTok, loc (peek state), cond, thenBlock, elseBranch) :> StatementNode)

    and private parseWhile state : StatementNode option =
        let startTok = advance state
        let cond = Pratt.parseExpression state 0
        match parseBlock state with
        | Some body -> Some (WhileStatementNode(loc startTok, loc (peek state), cond, body) :> StatementNode)
        | None -> None

    and private parseFor state : StatementNode option =
        let startTok = advance state
        if at state TokenType.LeftParen then
            parseForIn state startTok
        elif at state TokenType.Var then
            parseForCounting state startTok
        else
            state.Errors.Add(SyntaxError(SyntaxErrorKind.MissingVarOrParenAfterFor, "Expected 'var' or '(' after 'for'", loc (peek state)))
            None

    and private parseForIn state startTok : StatementNode option =
        advance state |> ignore // consume (
        match expect state TokenType.Identifier SyntaxErrorKind.MissingIdentifierInForIn "Expected identifier in for-in" with
        | None -> None
        | Some id ->
            if not (at state TokenType.In) then
                state.Errors.Add(SyntaxError(SyntaxErrorKind.MissingInInForIn, "Expected 'in' in for-in", loc (peek state)))
                None
            else
                advance state |> ignore
                let iter = Pratt.parseExpression state 0
                expect state TokenType.RightParen SyntaxErrorKind.MissingCloseParenInForIn "Expected ')' in for-in" |> ignore
                match parseBlock state with
                | Some body -> Some (ForInStatementNode(loc startTok, loc (peek state), id.Lexeme, iter, body) :> StatementNode)
                | None -> None

    and private parseForCounting state startTok : StatementNode option =
        advance state |> ignore // consume var
        match expect state TokenType.Identifier SyntaxErrorKind.MissingIdentifierInFor "Expected identifier in for" with
        | None -> None
        | Some id ->
            if not (at state TokenType.Equal) then
                state.Errors.Add(SyntaxError(SyntaxErrorKind.MissingEqualsInFor, "Expected '=' in for", loc (peek state)))
                None
            else
                advance state |> ignore
                let startExpr = Pratt.parseExpression state 0
                if not (at state TokenType.To) then
                    state.Errors.Add(SyntaxError(SyntaxErrorKind.MissingToInFor, "Expected 'to' in for", loc (peek state)))
                    None
                else
                    advance state |> ignore
                    let endExpr = Pratt.parseExpression state 0
                    match parseBlock state with
                    | Some body -> Some (ForStatementNode(loc startTok, loc (peek state), id.Lexeme, startExpr, endExpr, body) :> StatementNode)
                    | None -> None

    and private parseVarDecl state startToken isConst isPrivate : StatementNode option =
        advance state |> ignore // consume 'var'
        parseDeclarationTail state startToken isConst isPrivate
            SyntaxErrorKind.MissingIdentifierAfterVar "Expected identifier after 'var'"

    and private parseDeclarationTail state startToken isConst isPrivate idKind idMessage : StatementNode option =
        match expect state TokenType.Identifier idKind idMessage with
        | None -> None
        | Some id ->
            if not (at state TokenType.Equal) then
                state.Errors.Add(SyntaxError(SyntaxErrorKind.MissingEqualsInDeclaration, "Expected '=' in variable declaration", loc (peek state)))
                None
            else
                advance state |> ignore
                let init = Pratt.parseExpression state 0
                expect state TokenType.Semicolon SyntaxErrorKind.MissingSemicolonAfterDeclaration "Expected ';' after declaration" |> ignore
                // Untype-annotated declarations (var, inferred const) pass null
                // for C# nullable VariableDeclarationNode.Type.
                Some (VariableDeclarationNode(loc startToken, loc (peek state), null, isConst, id.Lexeme, init, isPrivate) :> StatementNode)

    and private parseTypedVarDecl state startToken isConst isPrivate : StatementNode option =
        match parseType state with
        | None -> None
        | Some typ ->
            match expect state TokenType.Identifier SyntaxErrorKind.MissingIdentifierAfterType "Expected identifier after type" with
            | None -> None
            | Some id ->
                if not (at state TokenType.Equal) then
                    state.Errors.Add(SyntaxError(SyntaxErrorKind.MissingEqualsInDeclaration, "Expected '=' in variable declaration", loc (peek state)))
                    None
                else
                    advance state |> ignore
                    let init = Pratt.parseExpression state 0
                    expect state TokenType.Semicolon SyntaxErrorKind.MissingSemicolonAfterDeclaration "Expected ';' after declaration" |> ignore
                    Some (VariableDeclarationNode(loc startToken, loc (peek state), typ, isConst, id.Lexeme, init, isPrivate) :> StatementNode)

    and private parseExprStmt state : StatementNode option =
        let startTok = peek state
        let expr = Pratt.parseExpression state 0
        if at state TokenType.Equal then
            // assignment-statement: identifier [ '[' expression ']' ] '=' expression ';'
            advance state |> ignore
            let value = Pratt.parseExpression state 0
            expect state TokenType.Semicolon SyntaxErrorKind.MissingSemicolonAfterExpression "Expected ';' after expression" |> ignore
            match expr with
            | :? QualifiedNameNode as qn ->
                Some (VariableAssignmentNode(qn.StartLocation, loc (peek state), qn.Name, null, value) :> StatementNode)
            | :? IndexExpressionNode as idx ->
                match idx.Target with
                | :? QualifiedNameNode as target ->
                    Some (VariableAssignmentNode(idx.StartLocation, loc (peek state), target.Name, idx.Index, value) :> StatementNode)
                | _ ->
                    state.Errors.Add(SyntaxError(SyntaxErrorKind.ExpectedExpression, "Invalid assignment target", idx.StartLocation))
                    Some (ExpressionStatementNode(loc startTok, loc (peek state), expr) :> StatementNode)
            // member-suffix assignment: self.count = 1;, c.count = 1; (docs/GRAMMAR.md §4.3).
            // Nothing resolves a member to an assignable field yet — semantics
            // rejects every instance — but the grammar already includes it, so
            // the parser accepts it now rather than needing another change later.
            | :? MemberAccessNode as access ->
                Some (MemberAssignmentNode(access.StartLocation, loc (peek state), access.Target, access.Member, value) :> StatementNode)
            | _ ->
                state.Errors.Add(SyntaxError(SyntaxErrorKind.ExpectedExpression, "Invalid assignment target", expr.StartLocation))
                Some (ExpressionStatementNode(loc startTok, loc (peek state), expr) :> StatementNode)
        else
            expect state TokenType.Semicolon SyntaxErrorKind.MissingSemicolonAfterExpression "Expected ';' after expression" |> ignore
            Some (ExpressionStatementNode(loc startTok, loc (peek state), expr) :> StatementNode)

    // --- Top-level ---

    // Soft keywords: 'standard', 'module', 'external', 'as' lex as ordinary
    // identifiers (they stay usable as names); only the token after 'import'
    // and the token after a module/external path treat them as keywords.
    let private isNamedToken (token: Token) (name: string) =
        token.Type = TokenType.Identifier && token.Lexeme = name

    // Error recovery: skip tokens until (and including) the next ';' so one
    // malformed statement cannot spin the parse loop.
    let private consumeThroughSemicolon state =
        while not (eof state) && not (at state TokenType.Semicolon) do
            advance state |> ignore
        if at state TokenType.Semicolon then advance state |> ignore

    let private parseExtern state : TopLevelNode =
        let startTok = advance state
        let parts = ResizeArray<string>()
        match expect state TokenType.Identifier SyntaxErrorKind.MissingIdentifierAfterExtern "Expected identifier after 'extern'" with
        | None -> ()
        | Some id ->
            parts.Add(id.Lexeme)
            while at state TokenType.Dot do
                advance state |> ignore
                match expect state TokenType.Identifier SyntaxErrorKind.MissingIdentifierAfterDot "Expected identifier after '.'" with
                | None -> ()
                | Some id2 -> parts.Add(id2.Lexeme)
        expect state TokenType.Semicolon SyntaxErrorKind.MissingSemicolonAfterDeclaration "Expected ';' after extern" |> ignore
        ExternNode(loc startTok, loc (peek state), parts)

    // import-statement = 'import' ( 'standard' | 'module' | 'external' )
    //                     qualified-name [ 'as' identifier ] ';'
    let rec private parseImport state : TopLevelNode =
        let startToken = advance state // consume 'import'
        let kind =
            let tok = peek state
            if isNamedToken tok "standard" then advance state |> ignore; Some ImportKind.Standard
            elif isNamedToken tok "module" then advance state |> ignore; Some ImportKind.Module
            elif isNamedToken tok "external" then advance state |> ignore; Some ImportKind.External
            else
                state.Errors.Add(SyntaxError(SyntaxErrorKind.ExpectedImportKind, "Expected 'standard', 'module', or 'external' after 'import'", loc tok))
                None
        match kind with
        | None ->
            consumeThroughSemicolon state
            ImportStatementNode(loc startToken, loc (peek state), ImportKind.Standard, ResizeArray<string>(), null)
        | Some k ->
            let parts = ResizeArray<string>()
            match expect state TokenType.Identifier SyntaxErrorKind.MissingImportName "Expected a name after 'import'" with
            | None -> ()
            | Some id ->
                parts.Add(id.Lexeme)
                while at state TokenType.Dot do
                    advance state |> ignore
                    match expect state TokenType.Identifier SyntaxErrorKind.MissingIdentifierAfterDot "Expected identifier after '.'" with
                    | None -> ()
                    | Some segment -> parts.Add(segment.Lexeme)
            // C# nullable ImportStatementNode.Alias: null without 'as'.
            // 'standard' imports bind the group's own names — an alias has
            // nothing to name, so only module/external imports accept 'as'.
            let mutable alias: string = null
            if isNamedToken (peek state) "as" then
                if k = ImportKind.Standard then
                    state.Errors.Add(SyntaxError(SyntaxErrorKind.UnexpectedAliasForStandardImport, "'as' only applies to module and external imports", loc (peek state)))
                advance state |> ignore
                match expect state TokenType.Identifier SyntaxErrorKind.MissingIdentifierAfterAs "Expected identifier after 'as'" with
                | Some a -> alias <- a.Lexeme
                | None -> ()
            expect state TokenType.Semicolon SyntaxErrorKind.MissingSemicolonAfterImport "Expected ';' after import" |> ignore
            ImportStatementNode(loc startToken, loc (peek state), k, parts, alias)

    and private parseFuncDecl state isPrivate : TopLevelNode =
        let startTok = advance state
        let name =
            match expect state TokenType.Identifier SyntaxErrorKind.MissingFunctionName "Expected function name" with
            | Some id -> id.Lexeme
            | None -> "???"
        expect state TokenType.LeftParen SyntaxErrorKind.MissingOpenParenAfterFunctionName "Expected '(' after function name" |> ignore
        let parameters = ResizeArray<ParameterDefinitionNode>()
        if not (at state TokenType.RightParen) then
            parseParam state |> Option.iter parameters.Add
            while at state TokenType.Comma do
                advance state |> ignore
                parseParam state |> Option.iter parameters.Add
        expect state TokenType.RightParen SyntaxErrorKind.MissingCloseParenAfterParameters "Expected ')' after parameters" |> ignore
        expect state TokenType.Colon SyntaxErrorKind.MissingColonBeforeReturnType "Expected ':' before return type" |> ignore
        let retType =
            match parseType state with
            | Some t -> t
            | None -> TypeNode(loc (peek state), loc (peek state), ScalarType.Void, false)
        let body =
            match parseBlock state with
            | Some b -> b
            | None -> BlockNode(loc (peek state), loc (peek state), ResizeArray<StatementNode>(), null)
        FunctionDeclarationNode(loc startTok, loc (peek state), name, parameters, retType, body, isPrivate)

    and private parseParam state : ParameterDefinitionNode option =
        match parseType state with
        | None -> None
        | Some typ ->
            match expect state TokenType.Identifier SyntaxErrorKind.MissingParameterName "Expected parameter name" with
            | None -> None
            | Some id -> Some (ParameterDefinitionNode(typ.StartLocation, loc id, typ, id.Lexeme))

    // --- Class files (docs/GRAMMAR.md §4) ---

    // modifier = 'private' | 'readonly' ; 'override'/'abstract'/'static' are
    // reserved but not supported yet — reported once per occurrence, then
    // skipped so the member after them still parses.
    and private parseModifiers state : bool * bool =
        let mutable isPrivate = false
        let mutable isReadonly = false
        let mutable loop = true
        while loop do
            match (peek state).Type with
            | TokenType.Private ->
                advance state |> ignore
                isPrivate <- true
            | TokenType.Readonly ->
                advance state |> ignore
                isReadonly <- true
            | TokenType.Override | TokenType.Abstract | TokenType.Static ->
                let tok = advance state
                state.Errors.Add(SyntaxError(SyntaxErrorKind.ReservedKeywordNotSupportedYet, sprintf "'%s' is reserved, not supported yet" tok.Lexeme, loc tok))
            | _ -> loop <- false
        (isPrivate, isReadonly)

    // Shared tail for 'field'/'const' once the keyword and (optional) type
    // are consumed: identifier [ '=' expression ] ';'. A const with no
    // initializer is a syntax error (a constant can't be assigned later to
    // give it one); a field's initializer may be omitted — semantics
    // enforces definite assignment (docs/GRAMMAR.md §4.1), not the parser.
    and private parseFieldTail state startTok (typ: TypeNode option) isConst isPrivate isReadonly : TopLevelNode option =
        match expect state TokenType.Identifier SyntaxErrorKind.MissingIdentifierAfterType "Expected identifier after type" with
        | None -> None
        | Some id ->
            let hasInit = at state TokenType.Equal
            if hasInit then advance state |> ignore
            if isConst && not hasInit then
                state.Errors.Add(SyntaxError(SyntaxErrorKind.MissingEqualsInDeclaration, "Expected '=' in constant declaration", loc (peek state)))
            let init = if hasInit then Some (Pratt.parseExpression state 0) else None
            expect state TokenType.Semicolon SyntaxErrorKind.MissingSemicolonAfterDeclaration "Expected ';' after declaration" |> ignore
            Some (FieldDeclarationNode(loc startTok, loc (peek state), Option.toObj typ, isConst, isPrivate, isReadonly, id.Lexeme, Option.toObj init) :> TopLevelNode)

    // field = 'field' ( type | 'var' ) identifier [ '=' expression ] ';'
    and private parseField state isPrivate isReadonly : TopLevelNode option =
        let startTok = advance state // consume 'field'
        if at state TokenType.Var then
            advance state |> ignore
            parseFieldTail state startTok None false isPrivate isReadonly
        else
            match parseType state with
            | None -> None
            | Some typ -> parseFieldTail state startTok (Some typ) false isPrivate isReadonly

    // const = 'const' [ type | 'var' ] identifier '=' expression ';'
    // (no 'field' keyword — a class constant reads the same as a module one)
    and private parseClassConst state isPrivate : TopLevelNode option =
        let startTok = advance state // consume 'const'
        if at state TokenType.Var then
            advance state |> ignore
            parseFieldTail state startTok None true isPrivate false
        else
            match (peek state).Type with
            | TokenType.Int | TokenType.Float | TokenType.String
            | TokenType.Char | TokenType.Bool | TokenType.Void | TokenType.Identifier ->
                match parseType state with
                | None -> None
                | Some typ -> parseFieldTail state startTok (Some typ) true isPrivate false
            | _ ->
                // No type or 'var': infer from the initializer, like a
                // top-level inferred const.
                parseFieldTail state startTok None true isPrivate false

    // constructor = 'constructor' '(' [ parameter-list ] ')' [ ':' delegate ] block ;
    // delegate    = 'self' '(' [ argument-list ] ')' ;  ('super(...)' reserved, 5)
    and private parseConstructor state isPrivate : TopLevelNode option =
        let startTok = advance state // consume 'constructor'
        expect state TokenType.LeftParen SyntaxErrorKind.MissingOpenParenAfterConstructor "Expected '(' after 'constructor'" |> ignore
        let parameters = ResizeArray<ParameterDefinitionNode>()
        if not (at state TokenType.RightParen) then
            parseParam state |> Option.iter parameters.Add
            while at state TokenType.Comma do
                advance state |> ignore
                parseParam state |> Option.iter parameters.Add
        expect state TokenType.RightParen SyntaxErrorKind.MissingCloseParenAfterConstructorParameters "Expected ')' after constructor parameters" |> ignore
        let delegateArgs =
            if at state TokenType.Colon then
                advance state |> ignore // consume ':'
                match (peek state).Type with
                | TokenType.Self ->
                    advance state |> ignore
                    let args = ResizeArray<ExpressionNode>()
                    if at state TokenType.LeftParen then
                        advance state |> ignore
                        if not (at state TokenType.RightParen) then
                            args.Add(Pratt.parseExpression state 0)
                            while at state TokenType.Comma do
                                advance state |> ignore
                                args.Add(Pratt.parseExpression state 0)
                        expect state TokenType.RightParen SyntaxErrorKind.MissingCloseParenAfterDelegateArguments "Expected ')' to close delegate arguments" |> ignore
                    else
                        state.Errors.Add(SyntaxError(SyntaxErrorKind.MissingOpenParenAfterDelegate, "Expected '(' after 'self'", loc (peek state)))
                    Some (args :> IReadOnlyList<ExpressionNode>)
                | TokenType.Super ->
                    let tok = advance state
                    state.Errors.Add(SyntaxError(SyntaxErrorKind.ReservedKeywordNotSupportedYet, "'super' is reserved, not supported yet — there is no 'extends' yet", loc tok))
                    None
                | _ ->
                    state.Errors.Add(SyntaxError(SyntaxErrorKind.MissingDelegateTargetAfterColon, "Expected 'self' after ':' in constructor", loc (peek state)))
                    None
            else
                None
        let body =
            match parseBlock state with
            | Some b -> b
            | None -> BlockNode(loc (peek state), loc (peek state), ResizeArray<StatementNode>(), null)
        Some (ConstructorDeclarationNode(loc startTok, loc (peek state), parameters, Option.toObj delegateArgs, body, isPrivate) :> TopLevelNode)

    // class-element = { modifier } ( field | method | constructor )
    //              | import-statement ;
    // (docs/GRAMMAR.md §4.4: same-folder declarations need no import, but a
    // class file can still import across folders, same as a module.)
    and private parseClassElement state : TopLevelNode option =
        if at state TokenType.Import then
            Some (parseImport state)
        else
            let isPrivate, isReadonly = parseModifiers state
            match (peek state).Type with
            | TokenType.Field -> parseField state isPrivate isReadonly
            | TokenType.Const -> parseClassConst state isPrivate
            | TokenType.Constructor -> parseConstructor state isPrivate
            | TokenType.Func -> Some (parseFuncDecl state isPrivate)
            | TokenType.Extends | TokenType.Implements ->
                let tok = advance state
                state.Errors.Add(SyntaxError(SyntaxErrorKind.ReservedKeywordNotSupportedYet, sprintf "'%s' is reserved, not supported yet" tok.Lexeme, loc tok))
                consumeThroughSemicolon state
                None
            | _ ->
                state.Errors.Add(SyntaxError(SyntaxErrorKind.MissingFieldOrMemberInClassFile, "Expected 'field', 'const', 'constructor', or a method in a class file", loc (peek state)))
                consumeThroughSemicolon state
                None

    // class-file = { class-element } ; — no loose statements (docs/GRAMMAR.md §4).
    and private parseClassFile state : TopLevelNode list =
        let members = ResizeArray<TopLevelNode>()
        let rec loop () =
            if not (eof state) then
                let posBefore = state.Pos
                parseClassElement state |> Option.iter members.Add
                // Forward-progress guard, same as the module top-level loop.
                if state.Pos = posBefore then advance state |> ignore
                loop ()
        loop ()
        List.ofSeq members

    let private parseTopLevel state : TopLevelNode list * StatementNode list =
        let members = ResizeArray<TopLevelNode>()
        let statements = ResizeArray<StatementNode>()
        let rec loop () =
            if not (eof state) then
                let posBefore = state.Pos
                // module-element = ... | [ 'private' ] function-declaration
                //                      | [ 'private' ] variable-declaration | ...
                // Every top-level declaration is public by default
                // (GRAMMAR.md §3.3); 'private' opts one out. A private
                // variable declaration is an executable top-level statement
                // (like any declaration); a private function is a
                // declaration proper — hence the split destination lists.
                match (peek state).Type with
                | TokenType.Extern -> members.Add(parseExtern state)
                | TokenType.Func -> members.Add(parseFuncDecl state false)
                | TokenType.Import -> members.Add(parseImport state)
                | TokenType.Private ->
                    let privateToken = advance state
                    match (peek state).Type with
                    | TokenType.Func -> members.Add(parseFuncDecl state true)
                    | TokenType.Var ->
                        advance state |> ignore // consume 'var'
                        parseDeclarationTail state privateToken false true
                            SyntaxErrorKind.MissingIdentifierAfterVar "Expected identifier after 'var'"
                        |> Option.iter statements.Add
                    | TokenType.Const ->
                        parseConstDecl state privateToken true |> Option.iter statements.Add
                    | TokenType.Int
                    | TokenType.Float
                    | TokenType.String
                    | TokenType.Char
                    | TokenType.Bool ->
                        parseTypedVarDecl state privateToken false true |> Option.iter statements.Add
                    | _ ->
                        state.Errors.Add(SyntaxError(SyntaxErrorKind.ExpectedDeclarationAfterPrivate, "Expected 'func', 'var', 'const', or a type after 'private'", loc (peek state)))
                        consumeThroughSemicolon state
                | _ -> parseStatement state |> Option.iter statements.Add
                // Forward-progress guard: any error path that consumed no
                // token would otherwise spin on the same offending token.
                if state.Pos = posBefore then advance state |> ignore
                loop ()
        loop ()
        List.ofSeq members, List.ofSeq statements

    // --- Entry point ---

    /// Parses a token stream into a <see cref="ParseResult"/>, per
    /// <paramref name="fileKind"/>'s grammar (docs/GRAMMAR.md §3-4).
    /// Interface files have no grammar yet — reserved (Phase 2 of
    /// docs/brainstorm/FLAT_PLAN.md).
    let Parse (lexResult: LexResult) (fileKind: PirateFileKind) : ParseResult =
        let state =
            { Tokens = lexResult.Tokens
              Pos = 0
              // Syntax errors only — lexer errors stay in LexResult.Errors;
              // the CLI concatenates both lists for display.
              Errors = ResizeArray<CompilationError>() }
        let members, statements =
            match fileKind with
            | PirateFileKind.Class -> parseClassFile state, ([]: StatementNode list)
            | PirateFileKind.Interface ->
                state.Errors.Add(SyntaxError(SyntaxErrorKind.ReservedKeywordNotSupportedYet, "Interface files are reserved, not supported yet", loc (peek state)))
                ([]: TopLevelNode list), ([]: StatementNode list)
            | _ -> parseTopLevel state
        let program =
            let bounds =
                [ for m in members -> (m.StartLocation, m.EndLocation)
                  for s in statements -> (s.StartLocation, s.EndLocation) ]
            match bounds with
            | [] -> None
            | _ ->
                let firstLoc = bounds |> List.map fst |> List.minBy (fun l -> (l.Line, l.Column))
                let lastLoc = bounds |> List.map snd |> List.maxBy (fun l -> (l.Line, l.Column))
                Some (ProgramNode(firstLoc, lastLoc, ResizeArray<TopLevelNode>(members), ResizeArray<StatementNode>(statements)))
        ParseResult(program, state.Errors)
