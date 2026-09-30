namespace Pirate.Syntax.Nodes;

/// <summary>
/// An array index operation: <c>list[i]</c>, <c>matrix[row][col]</c>, or
/// chained forms like <c>getArray()[0]</c>. <see cref="Target"/> is the
/// expression being indexed; <see cref="Index"/> is the index expression.
/// </summary>
public sealed record IndexExpressionNode(
    SourceLocation StartLocation,
    SourceLocation EndLocation,
    ExpressionNode Target,
    ExpressionNode Index)
    : ExpressionNode(StartLocation, EndLocation);
