namespace Pirate.Syntax.Nodes;

/// <summary>
/// A unary prefix operation: logical negation (<c>!</c>) or numeric
/// negation (<c>-</c>). Per the grammar (§3.5), these are right-associative,
/// so <c>!!x</c> and <c>--x</c> are valid.
/// </summary>
public sealed record UnaryOperationNode(
    SourceLocation StartLocation,
    SourceLocation EndLocation,
    UnaryOperator Operator,
    ExpressionNode Operand)
    : ExpressionNode(StartLocation, EndLocation);

public enum UnaryOperator
{
    Negate,  // -
    Not,     // !
}
