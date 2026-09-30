namespace Pirate.Syntax;

/// <summary>
/// A variable: <c>var</c>/<c>const</c>/explicitly typed declarations,
/// function parameters, and loop variables. Base for
/// <see cref="ImportedVariableSymbol"/>, which reuses this shape (and every
/// type-driven check it drives) for a variable imported from another
/// module.
/// </summary>
public record VariableSymbol(
    string Name,
    int Index,
    SymbolScope Scope,
    PirateType Type,
    bool IsConst)
    : Symbol(Name, Index, Scope);
