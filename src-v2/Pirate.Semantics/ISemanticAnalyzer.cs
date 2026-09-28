using Pirate.Syntax.Nodes;

namespace Pirate.Semantics;

/// <summary>
/// The semantics pipeline stage: name resolution and static type checking
/// over a parsed program, returning the checked AST plus every
/// <see cref="SemanticsError"/> found (collect-don't-throw — the stage
/// never raises for user errors).
/// </summary>
public interface ISemanticAnalyzer
{
    /// <summary>
    /// Runs the pass over a parsed program. Callers should only invoke this
    /// when the lexer and parser reported no errors — the analyzer assumes
    /// a well-formed AST, and cascading over partial parses would
    /// double-report.
    /// </summary>
    SemanticResult Analyze(ProgramNode program);
}
