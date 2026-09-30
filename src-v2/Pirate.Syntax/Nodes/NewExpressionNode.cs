using System.Collections.Generic;

namespace Pirate.Syntax.Nodes;

/// <summary>
/// An instance-creation expression: <c>new Counter(10)</c>, or a dotted
/// class name like <c>new shop.models.Money(0)</c> (docs/GRAMMAR.md
/// §3.6/§4). <see cref="ClassName"/> is the full dotted name as written —
/// resolving it against the project's type registry is Phase 3/4 of
/// docs/brainstorm/FLAT_PLAN.md.
/// </summary>
public sealed record NewExpressionNode(
    SourceLocation StartLocation,
    SourceLocation EndLocation,
    string ClassName,
    IReadOnlyList<ExpressionNode> Arguments)
    : ExpressionNode(StartLocation, EndLocation);
