namespace Pirate.Syntax;

/// <summary>
/// A module import that could not be resolved. <see cref="Reason"/> selects
/// the diagnostic the semantics pass reports; <see cref="Location"/> is the
/// external dependency's declared location (for location-based failures),
/// and <see cref="CyclePath"/> names the modules forming the cycle (for
/// <see cref="UnresolvedModuleReason.CyclicImport"/>, from the importing
/// module back to itself).
/// </summary>
public sealed record UnresolvedModuleImport(
    string ModuleName,
    UnresolvedModuleReason Reason,
    string? Location = null,
    IReadOnlyList<string>? CyclePath = null)
    : ModuleImportResolution;
