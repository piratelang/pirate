namespace Pirate.Syntax.Nodes;

/// <summary>
/// A variable assignment: <c>x = 5;</c> or an array element assignment
/// <c>list[i] = "ahoy";</c>. When <see cref="Index"/> is non-null, this is
/// an indexed assignment (the bracket syntax goes around the index expression).
/// </summary>
public sealed record VariableAssignmentNode(
    SourceLocation StartLocation,
    SourceLocation EndLocation,
    string Name,
    ExpressionNode? Index,
    ExpressionNode Value)
    : StatementNode(StartLocation, EndLocation);
