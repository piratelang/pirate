namespace Pirate.Syntax;

/// <summary>
/// A variable: <c>var</c>/<c>const</c>/explicitly typed declarations,
/// function parameters, and loop variables.
/// </summary>
public sealed record VariableSymbol(
    string Name,
    int Index,
    SymbolScope Scope,
    PirateType Type,
    bool IsConst)
    : Symbol(Name, Index, Scope);
