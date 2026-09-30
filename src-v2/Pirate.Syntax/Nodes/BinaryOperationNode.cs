namespace Pirate.Syntax.Nodes;

/// <summary>
/// A binary operation between two expressions, covering all operator
/// precedence levels from the grammar (§3.5):
/// arithmetic (+, -, *, /, %, ^), comparison (==, !=, &lt;, &lt;=, &gt;, &gt;=),
/// and logical (&&, ||).
/// </summary>
public sealed record BinaryOperationNode(
    SourceLocation StartLocation,
    SourceLocation EndLocation,
    ExpressionNode Left,
    BinaryOperator Operator,
    ExpressionNode Right)
    : ExpressionNode(StartLocation, EndLocation);

/// <summary>
/// The operator in a <see cref="BinaryOperationNode"/> — one spelling per
/// grammar production, mapped from <c>TokenType</c> by the parser.
/// </summary>
public enum BinaryOperator
{
    Add,        // +
    Subtract,   // -
    Multiply,   // *
    Divide,     // /
    Modulo,     // %
    Power,      // ^
    Equal,      // ==
    NotEqual,   // !=
    Less,       // <
    LessEqual,  // <=
    Greater,    // >
    GreaterEqual, // >=
    And,        // &&
    Or,         // ||
}
