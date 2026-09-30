namespace Pirate.Syntax.Nodes;

/// <summary>
/// <c>self</c> used as an expression — the current instance inside a class
/// file's constructor or method body (docs/GRAMMAR.md §3.6/§4). Never a
/// type; the type is always spelled by the class file's own name.
/// </summary>
public sealed record SelfExpressionNode(SourceLocation StartLocation, SourceLocation EndLocation)
    : ExpressionNode(StartLocation, EndLocation);
