namespace Pirate.Syntax.Nodes;

/// <summary>
/// A for-in loop: <c>for (item in items) { ... }</c>. The iterated
/// <see cref="Iterable"/> must type-check as <c>T[]</c> for some <c>T</c>;
/// <see cref="VariableName"/> is bound with type <c>T</c> for the loop body.
/// </summary>
public sealed record ForInStatementNode(
    SourceLocation StartLocation,
    SourceLocation EndLocation,
    string VariableName,
    ExpressionNode Iterable,
    BlockNode Body)
    : StatementNode(StartLocation, EndLocation);
