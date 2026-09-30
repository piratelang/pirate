using System.Collections.Generic;

namespace Pirate.Syntax.Nodes;

/// <summary>
/// A literal value: integer, float, string, char, bool, or null. The
/// <see cref="Value"/> property holds the parsed value (int, double,
/// string, char, or bool; always null for <see cref="LiteralKind.Null"/>)
/// and <see cref="LiteralKind"/> identifies which kind.
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

    /// <summary>The <c>null</c> literal (docs/GRAMMAR.md §2) — the empty value of any nullable type <c>T?</c>.</summary>
    Null,
}
