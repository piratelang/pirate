namespace Pirate.Syntax;

/// <summary>
/// A module import that resolved: <see cref="Exports"/> are the providing
/// module's exported symbols (its own <see cref="VariableSymbol"/>/
/// <see cref="FunctionSymbol"/> declarations, carrying that module's slot
/// indexes), which the semantics pass binds into the importer under the
/// import's alias. Only modules that checked clean provide an interface.
/// </summary>
public sealed record ResolvedModuleImport(
    string ModuleName,
    IReadOnlyList<Symbol> Exports)
    : ModuleImportResolution;
