namespace Pirate.Syntax;

/// <summary>
/// Why a module import could not be resolved to an interface. The module
/// linker (which knows the project's module graph) records one of these on
/// the import node; the semantics pass turns it into the matching
/// <see cref="SemanticsErrorKind"/> diagnostic.
/// </summary>
public enum UnresolvedModuleReason
{
    /// <summary>No module (or declared external dependency) with that name exists.</summary>
    NotFound,

    /// <summary>The external dependency's declared location does not exist on disk.</summary>
    LocationMissing,

    /// <summary>The external dependency's location is a remote URL; fetching is not implemented.</summary>
    RemoteLocation,

    /// <summary>The import participates in a cycle; no dependency order can provide its interface.</summary>
    CyclicImport,

    /// <summary>The imported module carries top-level executable statements, which only the entry module may run.</summary>
    HasTopLevelStatements,

    /// <summary>The imported module failed to lex, parse, or type-check, so it provides no interface.</summary>
    HasErrors,
}
