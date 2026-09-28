using System.Collections.Generic;
using Pirate.Syntax;

namespace Pirate.Semantics;

/// <summary>
/// One scope's name→symbol map, chained through <see cref="Parent"/> to the
/// global scope. Lives in the semantics pass, not the compiler (v2-012): the
/// analyzer builds a table per function scope, resolves every name to a
/// <see cref="Symbol"/> with a slot index, and the compiler later emits
/// index-based opcodes from those annotations.
/// </summary>
public sealed class SymbolTable
{
    private readonly Dictionary<string, Symbol> _symbols = new(StringComparer.Ordinal);

    public SymbolTable? Parent { get; }

    /// <summary>
    /// Nesting depth; the global table is level 0.
    /// </summary>
    public int ScopeLevel { get; }

    /// <summary>
    /// Number of symbols defined in this table — doubles as the slot index
    /// the next definition will receive.
    /// </summary>
    public int Count => _symbols.Count;

    public SymbolTable(SymbolTable? parent = null)
    {
        Parent = parent;
        ScopeLevel = parent is null ? 0 : parent.ScopeLevel + 1;
    }

    /// <summary>
    /// Defines <paramref name="symbol"/> in this scope only. Returns null on
    /// success, or the pre-existing symbol on a name collision (caller turns
    /// that into a duplicate-declaration error).
    /// </summary>
    public Symbol? Define(Symbol symbol)
    {
        if (_symbols.TryGetValue(symbol.Name, out var existing))
        {
            return existing;
        }

        _symbols[symbol.Name] = symbol;
        return null;
    }

    /// <summary>
    /// Resolves a name in this scope, then outward through parents to the
    /// global scope. Returns null when undeclared.
    /// </summary>
    public Symbol? Resolve(string name)
    {
        for (var table = this; table is not null; table = table.Parent)
        {
            if (table._symbols.TryGetValue(name, out var symbol))
            {
                return symbol;
            }
        }

        return null;
    }
}
