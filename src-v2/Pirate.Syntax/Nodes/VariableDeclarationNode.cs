namespace Pirate.Syntax.Nodes;

/// <summary>
/// A variable declaration: <c>int x = 5;</c>, <c>var name = "pirate";</c>, or
/// <c>int[] list = [];</c>. The <see cref="Type"/> is null only for
/// <c>var</c> declarations (the type is inferred from the initializer).
/// </summary>
public sealed record VariableDeclarationNode(
    SourceLocation StartLocation,
    SourceLocation EndLocation,
    TypeNode? Type,
    string Name,
    ExpressionNode Initializer)
    : StatementNode(StartLocation, EndLocation);
