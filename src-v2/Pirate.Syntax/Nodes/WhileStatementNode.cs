namespace Pirate.Syntax.Nodes;

/// <summary>
/// A while loop: <c>while x &lt; 5 { ... }</c>. No parentheses required
/// around the condition (matching v1), though parenthesized expressions are
/// still valid since <c>( expr )</c> is a primary expression.
/// </summary>
public sealed record WhileStatementNode(
    SourceLocation StartLocation,
    SourceLocation EndLocation,
    ExpressionNode Condition,
    BlockNode Body)
    : StatementNode(StartLocation, EndLocation);
