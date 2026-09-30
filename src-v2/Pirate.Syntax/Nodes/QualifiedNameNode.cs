namespace Pirate.Syntax.Nodes;

/// <summary>
/// A bare identifier, such as <c>counter</c> or <c>Standard</c>. A dotted
/// chain (<c>Standard.Terminal.Print</c>) is no longer one node — it's
/// nested <see cref="MemberAccessNode"/>s built by the postfix parser
/// (docs/GRAMMAR.md §3.6), with this node as the innermost target.
/// </summary>
public sealed record QualifiedNameNode(
    SourceLocation StartLocation,
    SourceLocation EndLocation,
    string Name)
    : ExpressionNode(StartLocation, EndLocation)
{
    /// <summary>
    /// The variable/function/builtin this name resolves to, set by the
    /// semantics pass. Null until resolution.
    /// </summary>
    public Symbol? ResolvedSymbol { get; set; }
}
