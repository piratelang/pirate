namespace Pirate.Syntax.Nodes;

/// <summary>
/// A function parameter: <c>int x</c>, <c>string name</c>, etc. The
/// <see cref="Type"/> is always present — parameters are always typed.
/// </summary>
public sealed record ParameterDefinitionNode(
    SourceLocation StartLocation,
    SourceLocation EndLocation,
    TypeNode Type,
    string Name)
    : StatementNode(StartLocation, EndLocation)
{
    /// <summary>
    /// The local this parameter binds, set by the semantics pass.
    /// </summary>
    public VariableSymbol? ResolvedSymbol { get; set; }
}
