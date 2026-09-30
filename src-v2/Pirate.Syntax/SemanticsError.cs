namespace Pirate.Syntax;

/// <summary>
/// Fine-grained semantic error kinds. Each value maps to exactly one
/// <c>SEM-xxx</c> error code in the CLI's <c>ErrorMapper</c>.
/// </summary>
public enum SemanticsErrorKind
{
    UndeclaredVariable,
    UndeclaredFunction,
    TypeMismatch,
    AssignmentToConst,
    MissingReturnInNonVoidFunction,
    VoidFunctionReturnsValue,
    ReturnValueRequired,
    EmptyArrayRequiresElementType,
    ForInIterableMustBeArray,
    DuplicateDeclaration,
    UnknownExtern,
    UnknownImport,
    ReturnAtTopLevel,

    // Module linking (docs/brainstorm/FLAT_PLAN.md Phase 3/4) — replaces
    // the retired ModuleImportUnsupported now that cross-file resolution
    // exists.
    UnknownModule,
    UnknownDependency,
    DependencyLocationMissing,
    RemoteDependencyUnsupported,
    CyclicModuleImport,
    ImportedModuleHasTopLevelCode,
    ImportedModuleHasErrors,
}

/// <summary>
/// A semantic error reported by <c>Pirate.Semantics</c>.
/// </summary>
public sealed class SemanticsError : CompilationError
{
    public SemanticsErrorKind Kind { get; }

    public SemanticsError(SemanticsErrorKind kind, string message, SourceLocation startLocation, SourceLocation? endLocation = null)
        : base(message, startLocation, endLocation)
    {
        Kind = kind;
    }
}
