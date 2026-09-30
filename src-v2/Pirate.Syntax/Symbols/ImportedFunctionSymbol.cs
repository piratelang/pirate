namespace Pirate.Syntax;

/// <summary>
/// An exported function of another module, bound into the importing module
/// under its alias-qualified name (<c>Data.greet</c>). Derives from
/// <see cref="FunctionSymbol"/> so call checking (parameter types, return
/// type, arity) works unchanged; <see cref="ModuleName"/> plus the
/// <see cref="SymbolScope.Module"/> scope tell the compiler the slot
/// <see cref="Symbol.Index"/> is the function's index in the *providing*
/// module, so a call is a cross-module call, not a local one.
/// </summary>
public sealed record ImportedFunctionSymbol(
    string Name,
    int Index,
    IReadOnlyList<PirateType> Parameters,
    PirateType ReturnType,
    string ModuleName)
    : FunctionSymbol(Name, Index, SymbolScope.Module, Parameters, ReturnType);
