using System.Collections.Generic;

namespace Pirate.Syntax.Nodes;

/// <summary>
/// The root of a parsed program. A valid program must define a zero-parameter
/// <c>func main() : void { ... }</c> as its entry point (grammar §3.1).
/// </summary>
public sealed record ProgramNode(
    SourceLocation StartLocation,
    SourceLocation EndLocation,
    IReadOnlyList<TopLevelNode> Members)
    : TopLevelNode(StartLocation, EndLocation);
