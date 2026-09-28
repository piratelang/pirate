using System.Collections.Generic;

namespace Pirate.Syntax.Nodes;

/// <summary>
/// The root of a parsed module: declarations (imports, externs, functions)
/// as <see cref="Members"/> and top-level executable code as
/// <see cref="Statements"/>. Only the run entry-point module — the one the
/// fleet resolves for <c>pirate run</c> — may carry top-level statements;
/// helper modules consist of declarations alone.
/// </summary>
public sealed record ProgramNode(
    SourceLocation StartLocation,
    SourceLocation EndLocation,
    IReadOnlyList<TopLevelNode> Members,
    IReadOnlyList<StatementNode> Statements)
    : TopLevelNode(StartLocation, EndLocation);
