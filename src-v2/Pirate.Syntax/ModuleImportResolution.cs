namespace Pirate.Syntax;

/// <summary>
/// The module linker's verdict for one <c>import module</c>/<c>import
/// external</c> statement, recorded on the import node before the semantics
/// pass runs: either the imported module's export interface
/// (<see cref="ResolvedModuleImport"/>) or the reason no interface exists
/// (<see cref="UnresolvedModuleImport"/>). Data only — the linker computes
/// it, the analyzer consumes it and binds names or reports the matching
/// <c>SemanticsError</c>.
/// </summary>
public abstract record ModuleImportResolution;
