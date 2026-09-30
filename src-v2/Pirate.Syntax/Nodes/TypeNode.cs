namespace Pirate.Syntax.Nodes;

/// <summary>
/// A type reference in a declaration: <c>int</c>, <c>string</c>,
/// <c>int[]</c>, <c>void</c> (return types only), etc. The grammar also
/// defines <c>T?</c> and a class-name base (docs/GRAMMAR.md §2, §4), via
/// <see cref="IsNullable"/> and <see cref="ClassName"/> — the parser
/// doesn't produce either yet (Phase 2 of
/// docs/brainstorm/FLAT_PLAN.md), so every node today has
/// <c>IsNullable: false, ClassName: null</c>.
/// </summary>
public sealed record TypeNode(
    SourceLocation StartLocation,
    SourceLocation EndLocation,
    ScalarType ScalarType,
    bool IsArray,
    bool IsNullable = false,
    string? ClassName = null)
{
    /// <summary>
    /// A shorthand for a non-array scalar type, since that's the common case.
    /// </summary>
    public static TypeNode Scalar(ScalarType type, SourceLocation location) =>
        new(location, location, type, IsArray: false);
}

/// <summary>
/// The six scalar types defined by the grammar (§2):
/// <c>int</c>, <c>float</c>, <c>string</c>, <c>char</c>, <c>bool</c>,
/// and <c>void</c> (return types only).
/// </summary>
public enum ScalarType
{
    Int,
    Float,
    String,
    Char,
    Bool,
    Void,
}
