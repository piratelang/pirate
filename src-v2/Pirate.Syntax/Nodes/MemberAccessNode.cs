namespace Pirate.Syntax.Nodes;

/// <summary>
/// A member access: <c>c.value</c>, or one segment of a dotted chain like
/// <c>Standard.Terminal.Print</c> (parsed as nested member accesses —
/// <c>MemberAccessNode(MemberAccessNode(Standard, Terminal), Print)</c>,
/// docs/GRAMMAR.md §3.6/§4.3). <see cref="Target"/> is the expression the
/// member is read from.
/// </summary>
public sealed record MemberAccessNode(
    SourceLocation StartLocation,
    SourceLocation EndLocation,
    ExpressionNode Target,
    string Member)
    : ExpressionNode(StartLocation, EndLocation)
{
    /// <summary>
    /// The variable/function/builtin this access resolves to, set by the
    /// semantics pass when the whole chain up to here names one dotted
    /// symbol path (docs/GRAMMAR.md §4.4's "longest namespace prefix, then
    /// type, then member") — e.g. <c>Standard.Terminal.Print</c>. Null
    /// until resolution, and stays null for a genuine object field/method
    /// access once those exist (Phase 4 of docs/brainstorm/FLAT_PLAN.md).
    /// </summary>
    public Symbol? ResolvedSymbol { get; set; }
}
