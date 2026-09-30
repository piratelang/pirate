using System.Collections.Generic;

namespace Pirate.Syntax.Nodes;

/// <summary>
/// A function call: <c>Print("Hello")</c>, <c>Standard.Terminal.Print(x)</c>,
/// or a chained call like <c>f()[0]</c>. The <see cref="Callee"/> is the
/// expression being called — a <see cref="QualifiedNameNode"/> for a bare
/// name, nested <see cref="MemberAccessNode"/>s for a dotted path, or (an
/// error today) the result of an index or another call.
/// </summary>
public sealed record FunctionCallNode(
    SourceLocation StartLocation,
    SourceLocation EndLocation,
    ExpressionNode Callee,
    IReadOnlyList<ExpressionNode> Arguments)
    : ExpressionNode(StartLocation, EndLocation)
{
    /// <summary>
    /// The function or builtin being called, set by the semantics pass.
    /// </summary>
    public Symbol? ResolvedCallee { get; set; }
}
