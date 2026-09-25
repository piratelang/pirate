namespace Pirate.Syntax.Nodes;

/// <summary>
/// A return statement: <c>return 42;</c> or <c>return;</c> (void functions
/// only). <see cref="Value"/> is null for void returns.
/// </summary>
public sealed record ReturnStatementNode(
    SourceLocation StartLocation,
    SourceLocation EndLocation,
    ExpressionNode? Value)
    : StatementNode(StartLocation, EndLocation);
