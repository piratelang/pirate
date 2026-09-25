namespace Pirate.Syntax.Nodes;

/// <summary>
/// A counting for loop: <c>for var i = 0 to 10 { ... }</c>. The loop
/// variable <see cref="VariableName"/> is implicitly declared with type
/// inferred from <see cref="Start"/> (must be <c>int</c>).
/// </summary>
public sealed record ForStatementNode(
    SourceLocation StartLocation,
    SourceLocation EndLocation,
    string VariableName,
    ExpressionNode Start,
    ExpressionNode End,
    BlockNode Body)
    : StatementNode(StartLocation, EndLocation);
