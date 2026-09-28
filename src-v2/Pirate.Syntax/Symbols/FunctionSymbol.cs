using System.Collections.Generic;

namespace Pirate.Syntax;

/// <summary>
/// A user-declared function.
/// </summary>
public sealed record FunctionSymbol(
    string Name,
    int Index,
    SymbolScope Scope,
    IReadOnlyList<PirateType> Parameters,
    PirateType ReturnType)
    : Symbol(Name, Index, Scope);
