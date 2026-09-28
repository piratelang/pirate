using System.Collections.Generic;

namespace Pirate.Syntax.Nodes;

/// <summary>
/// A dotted identifier chain such as <c>Standard.Terminal.Print</c> or a
/// bare identifier like <c>counter</c>. Produced when the parser encounters
/// an identifier (optionally followed by <c>.identifier</c> segments) that
/// is not immediately followed by <c>(</c> (which would make it a call) or
/// <c>[</c> (which would make it an index).
/// </summary>
public sealed record QualifiedNameNode(
    SourceLocation StartLocation,
    SourceLocation EndLocation,
    IReadOnlyList<string> Parts)
    : ExpressionNode(StartLocation, EndLocation)
{
    /// <summary>
    /// The variable/function/builtin this name resolves to, set by the
    /// semantics pass. Null until resolution.
    /// </summary>
    public Symbol? ResolvedSymbol { get; set; }
}
