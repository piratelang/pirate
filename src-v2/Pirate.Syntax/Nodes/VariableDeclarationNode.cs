namespace Pirate.Syntax.Nodes;

/// <summary>
/// A variable declaration: <c>int x = 5;</c>, <c>var name = "pirate";</c>,
/// <c>const int limit = 10;</c>, or <c>int[] list = [];</c>. The
/// <see cref="Type"/> is null for <c>var</c> declarations and for
/// type-less <c>const</c> declarations (the type is inferred from the
/// initializer). <see cref="IsConst"/> marks a <c>const</c>-declared
/// variable, which semantics rejects as an assignment target.
/// <see cref="IsExported"/> marks a variable declared with
/// <c>export var</c> (top level only); its visibility is consumed by the
/// module linker once that lands.
/// </summary>
public sealed record VariableDeclarationNode(
    SourceLocation StartLocation,
    SourceLocation EndLocation,
    TypeNode? Type,
    bool IsConst,
    string Name,
    ExpressionNode Initializer,
    bool IsExported)
    : StatementNode(StartLocation, EndLocation)
{
    /// <summary>
    /// The variable this declaration introduces, set by the semantics pass.
    /// </summary>
    public VariableSymbol? ResolvedSymbol { get; set; }
}
