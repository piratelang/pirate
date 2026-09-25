using System.Collections.Generic;

namespace Pirate.Syntax.Nodes;

/// <summary>
/// An array literal: <c>[1, 2, 3]</c> or <c>[]</c> (empty, only valid when
/// the element type is known from the declaration context — see
/// docs/GRAMMAR.md §2).
/// </summary>
public sealed record ArrayLiteralNode(
    SourceLocation StartLocation,
    SourceLocation EndLocation,
    IReadOnlyList<ExpressionNode> Elements)
    : ExpressionNode(StartLocation, EndLocation);
