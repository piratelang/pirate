namespace Pirate.Syntax.Nodes;

/// <summary>
/// An if/else statement. <see cref="Condition"/> is the guard expression
/// (must type-check as <c>bool</c>). <see cref="ThenBranch"/> is the block
/// executed when the condition is true. <see cref="ElseBranch"/> is present
/// for <c>else { ... }</c> and <c>else if ...</c> (where it wraps another
/// <see cref="IfStatementNode"/>).
/// </summary>
public sealed record IfStatementNode(
    SourceLocation StartLocation,
    SourceLocation EndLocation,
    ExpressionNode Condition,
    BlockNode ThenBranch,
    StatementNode? ElseBranch)
    : StatementNode(StartLocation, EndLocation);
