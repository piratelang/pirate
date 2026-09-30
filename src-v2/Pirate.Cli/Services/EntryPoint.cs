using System.Linq;
using Pirate.Syntax.Nodes;

namespace Pirate.Cli.Services;

/// <summary>
/// Run-entry rule: only the fleet-resolved entry-point module executes, and
/// what executes is its top-level statements. A module with none has
/// nothing to run. Helper modules may still declare functions, imports,
/// exports, and (until the module linker enforces entry-only code) top-level
/// statements for their own sake. Formalizing "nothing to run" as RTN-004
/// lands with the VM milestone.
/// </summary>
public static class EntryPoint
{
    /// <summary>
    /// True when the program has a run body: at least one top-level
    /// statement.
    /// </summary>
    public static bool HasRunnableBody(ProgramNode? program) =>
        program?.Statements.Count > 0;
}
