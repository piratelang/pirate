namespace Pirate.Syntax;

/// <summary>
/// Scope tiers a resolved symbol can live in, mirroring the v2 symbol-table
/// design: <see cref="Global"/> for module-level names (functions, imports,
/// top-level variables), <see cref="Local"/> for a function's parameters,
/// locals and loop variables, <see cref="Builtin"/> for standard-library
/// functions brought in by <c>extern</c>, <see cref="Module"/> for an
/// <c>import module</c>/<c>import external</c> alias and the exports
/// reached through it (docs/GRAMMAR.md §3.2/§4.4) — the symbol's
/// <c>Index</c> is a slot in the *providing* module, not the importer's own
/// globals — and <see cref="Free"/> reserved for closure capture (compiler
/// growth plan phase 9 — nothing resolves to it yet).
/// </summary>
public enum SymbolScope
{
    Global,
    Local,
    Builtin,
    Module,
    Free,
}
