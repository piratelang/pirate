namespace Pirate.Syntax;

/// <summary>
/// Scope tiers a resolved symbol can live in, mirroring the v2 symbol-table
/// design: <see cref="Global"/> for module-level names (functions, imports,
/// top-level variables), <see cref="Local"/> for a function's parameters,
/// locals and loop variables, <see cref="Builtin"/> for standard-library
/// functions brought in by <c>extern</c>, and <see cref="Free"/> reserved
/// for closure capture (compiler growth plan phase 9 — nothing resolves to
/// it yet).
/// </summary>
public enum SymbolScope
{
    Global,
    Local,
    Builtin,
    Free,
}
