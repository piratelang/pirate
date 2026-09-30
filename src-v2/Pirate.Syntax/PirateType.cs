using Pirate.Syntax.Nodes;

namespace Pirate.Syntax;

/// <summary>
/// A resolved type value in the semantic model: a scalar kind or (not yet
/// produced by anything — docs/GRAMMAR.md §4, Phase 4 of
/// docs/brainstorm/FLAT_PLAN.md) a class name, optionally nullable and
/// optionally a single-dimensional array. Distinct from <c>TypeNode</c>,
/// which is source syntax with a location; <c>PirateType</c> is the
/// location-free value the semantics pass computes and the compiler
/// consumes.
/// </summary>
/// <param name="Scalar">
/// Meaningful only when <see cref="ClassName"/> is null — every type today
/// is scalar, so existing code reads this unconditionally.
/// </param>
/// <param name="ClassName">
/// Null for every scalar type (all current usage). Set instead of
/// <paramref name="Scalar"/> once class types exist; see
/// <see cref="IsClassType"/>.
/// </param>
/// <param name="IsNullable">
/// Whether this is the type's nullable form, <c>T?</c> (docs/GRAMMAR.md
/// §2). Distinguishing <c>T?[]</c> (nullable elements) from <c>T[]?</c>
/// (a nullable array) is deferred until nullable types are actually
/// produced (Phase 2/4) — today this is always <c>false</c>.
/// </param>
public readonly record struct PirateType(ScalarType Scalar, bool IsArray, bool IsNullable = false, string? ClassName = null)
{
    public static PirateType Of(ScalarType scalar) => new(scalar, false);

    public static PirateType ArrayOf(ScalarType scalar) => new(scalar, true);

    public static readonly PirateType Int = Of(ScalarType.Int);
    public static readonly PirateType Float = Of(ScalarType.Float);
    public static readonly PirateType String = Of(ScalarType.String);
    public static readonly PirateType Char = Of(ScalarType.Char);
    public static readonly PirateType Bool = Of(ScalarType.Bool);
    public static readonly PirateType Void = Of(ScalarType.Void);

    /// <summary>A named class type (docs/GRAMMAR.md §4). Not yet produced.</summary>
    public static PirateType OfClass(string className) => new(default, false, ClassName: className);

    /// <summary>Whether this is a class type rather than a scalar one.</summary>
    public bool IsClassType => ClassName is not null;

    /// <summary>This type's nullable form, <c>T?</c>.</summary>
    public PirateType AsNullable() => this with { IsNullable = true };

    /// <summary>
    /// Element type of an array type, or null when this is not an array.
    /// </summary>
    public PirateType? ElementType => IsArray ? new PirateType(Scalar, false, ClassName: ClassName) : null;

    public bool IsNumeric => !IsArray && !IsClassType && Scalar is ScalarType.Int or ScalarType.Float;

    public override string ToString()
    {
        var name = ClassName ?? Scalar switch
        {
            ScalarType.Int => "int",
            ScalarType.Float => "float",
            ScalarType.String => "string",
            ScalarType.Char => "char",
            ScalarType.Bool => "bool",
            ScalarType.Void => "void",
            _ => Scalar.ToString().ToLowerInvariant(),
        };
        var nullSuffix = IsNullable ? "?" : "";
        return IsArray ? $"{name}[]{nullSuffix}" : $"{name}{nullSuffix}";
    }
}
