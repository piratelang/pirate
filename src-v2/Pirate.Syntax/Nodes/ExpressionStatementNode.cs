namespace Pirate.Syntax.Nodes;

/// <summary>
/// An expression used as a statement: <c>Print("Hello");</c> or
/// <c>x + 1;</c> (the latter would be a type error, but the parser still
/// produces it — semantics catches it).
/// </summary>
public sealed record ExpressionStatementNode(
    SourceLocation StartLocation,
    SourceLocation EndLocation,
    ExpressionNode Expression)
    : StatementNode(StartLocation, EndLocation);
