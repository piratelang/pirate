using System.Collections.Generic;

namespace Pirate.Syntax.Nodes;

/// <summary>
/// A literal value: integer, float, string, char, or bool. The <see cref="Value"/>
/// property holds the parsed value (int, double, string, char, or bool) and
/// <see cref="LiteralKind"/> identifies which kind.
/// </summary>
public sealed record LiteralNode(SourceLocation StartLocation, SourceLocation EndLocation, LiteralKind LiteralKind, object? Value)
    : ExpressionNode(StartLocation, EndLocation);

/// <summary>
/// Which kind of literal a <see cref="LiteralNode"/> represents.
/// </summary>
public enum LiteralKind
{
    Int,
    Float,
    String,
    Char,
    Bool,
}
