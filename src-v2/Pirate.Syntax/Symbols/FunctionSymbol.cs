using System.Collections.Generic;

namespace Pirate.Syntax;

/// <summary>
/// A user-declared function. Base for <see cref="ImportedFunctionSymbol"/>,
/// which reuses this shape (and every call-checking rule it drives) for a
/// function imported from another module.
/// </summary>
public record FunctionSymbol(
    string Name,
    int Index,
    SymbolScope Scope,
    IReadOnlyList<PirateType> Parameters,
    PirateType ReturnType)
    : Symbol(Name, Index, Scope);
