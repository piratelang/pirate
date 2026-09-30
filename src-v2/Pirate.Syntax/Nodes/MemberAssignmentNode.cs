namespace Pirate.Syntax.Nodes;

/// <summary>
/// An assignment through member access: <c>self.count = 1;</c>,
/// <c>c.count = 1;</c> (docs/GRAMMAR.md §4.3). Parsed now so Phase 4's
/// semantics (docs/brainstorm/FLAT_PLAN.md) doesn't also need a parser
/// change — today nothing resolves a member target to an assignable field,
/// so the analyzer rejects every instance of this node.
/// </summary>
public sealed record MemberAssignmentNode(
    SourceLocation StartLocation,
    SourceLocation EndLocation,
    ExpressionNode Target,
    string Member,
    ExpressionNode Value)
    : StatementNode(StartLocation, EndLocation);
