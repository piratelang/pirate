namespace Pirate.Syntax;

/// <summary>
/// An exported variable of another module, bound into the importing module
/// under its alias-qualified name (<c>Data.data</c>). Derives from
/// <see cref="VariableSymbol"/> so every type-driven check treats it exactly
/// like a local variable; <see cref="ModuleName"/> plus the
/// <see cref="SymbolScope.Module"/> scope tell the compiler the slot
/// <see cref="Symbol.Index"/> lives in the *providing* module's globals, so
/// reading it is a cross-module access, never a <c>LOAD_GLOBAL</c> into the
/// importing module's own slot array.
/// </summary>
public sealed record ImportedVariableSymbol(
    string Name,
    int Index,
    PirateType Type,
    bool IsConst,
    string ModuleName)
    : VariableSymbol(Name, Index, SymbolScope.Module, Type, IsConst);
