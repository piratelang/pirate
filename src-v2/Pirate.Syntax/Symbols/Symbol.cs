namespace Pirate.Syntax;

/// <summary>
/// A name resolved by the semantics pass. <see cref="Index"/> is the slot
/// within the symbol's scope (global slot, local slot, or builtin registry
/// position) that the compiler later encodes into index-based opcodes such
/// as <c>LOAD_GLOBAL 3</c> instead of a name lookup.
/// </summary>
public abstract record Symbol(string Name, int Index, SymbolScope Scope);
