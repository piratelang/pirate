namespace Pirate.Syntax.Nodes;

/// <summary>
/// A type reference in a declaration: <c>int</c>, <c>string</c>,
/// <c>int[]</c>, <c>void</c> (return types only), etc.
/// </summary>
public sealed record TypeNode(
    SourceLocation StartLocation,
    SourceLocation EndLocation,
    ScalarType ScalarType,
    bool IsArray)
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
