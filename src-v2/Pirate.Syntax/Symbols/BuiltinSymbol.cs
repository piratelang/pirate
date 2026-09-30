using System.Collections.Generic;

namespace Pirate.Syntax;

/// <summary>
/// A standard-library function brought in by an <c>extern</c> declaration
/// and known to the builtin registry.
/// </summary>
public sealed record BuiltinSymbol(
    string Name,
    int Index,
    SymbolScope Scope,
    IReadOnlyList<PirateType> Parameters,
    PirateType ReturnType)
    : Symbol(Name, Index, Scope);
