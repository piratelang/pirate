namespace Pirate.Syntax;

/// <summary>
/// The alias an <c>import module</c>/<c>import external</c> statement binds
/// into the importing module's global scope (e.g. <c>Data</c> for
/// <c>import module data as Data;</c>). The alias itself is not a value —
/// exports are reached through it as <c>&lt;Alias&gt;.&lt;name&gt;</c>, which
/// resolve to <see cref="ImportedVariableSymbol"/>/
/// <see cref="ImportedFunctionSymbol"/> bindings. <see cref="ModuleName"/>
/// is the dotted name of the providing module (a project module path or an
/// external dependency name).
/// </summary>
public sealed record ModuleSymbol(
    string Name,
    int Index,
    SymbolScope Scope,
    string ModuleName)
    : Symbol(Name, Index, Scope);
