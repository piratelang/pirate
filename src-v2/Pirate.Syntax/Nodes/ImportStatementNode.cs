using System.Collections.Generic;

namespace Pirate.Syntax.Nodes;

/// <summary>
/// What an <c>import</c> statement brings in (GRAMMAR.md §3.2):
/// <see cref="Standard"/> binds one builtin namespace group by its leaf
/// names; <see cref="Module"/> imports another <c>.pirate</c> module of the
/// project; <see cref="External"/> imports a module fetched from outside
/// the project. Module and external imports parse today but are rejected
/// by semantics until the module linker lands.
/// </summary>
public enum ImportKind
{
    Standard,
    Module,
    External,
}

/// <summary>
/// An import statement: <c>import standard Terminal;</c>,
/// <c>import module data as Data;</c>, or
/// <c>import external data as Data;</c>. The last path segment is the
/// default binding name when <see cref="Alias"/> is absent.
/// </summary>
public sealed record ImportStatementNode(
    SourceLocation StartLocation,
    SourceLocation EndLocation,
    ImportKind Kind,
    IReadOnlyList<string> Path,
    string? Alias)
    : TopLevelNode(StartLocation, EndLocation)
{
    /// <summary>
    /// Set by the semantics pass for <see cref="ImportKind.Standard"/>
    /// imports: the dotted namespace root the group resolved against, e.g.
    /// <c>Standard.Terminal</c>. Null before it runs.
    /// </summary>
    public string? ResolvedNamespace { get; set; }

    /// <summary>
    /// Set by the module linker for <see cref="ImportKind.Module"/> and
    /// <see cref="ImportKind.External"/> imports before the semantics pass
    /// runs: the resolved export interface, or the reason none exists. Null
    /// when no linker ran (a standalone single-module check), which the
    /// analyzer reports as an unknown module/dependency.
    /// </summary>
    public ModuleImportResolution? Resolution { get; set; }
}
