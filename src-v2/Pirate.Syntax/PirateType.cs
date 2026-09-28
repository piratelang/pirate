using Pirate.Syntax.Nodes;

namespace Pirate.Syntax;

/// <summary>
/// A resolved type value in the semantic model: one scalar kind, optionally
/// as a single-dimensional array. Distinct from <c>TypeNode</c>, which is
/// source syntax with a location; <c>PirateType</c> is the location-free
/// value the semantics pass computes and the compiler consumes.
/// </summary>
public readonly record struct PirateType(ScalarType Scalar, bool IsArray)
{
    public static PirateType Of(ScalarType scalar) => new(scalar, false);

    public static PirateType ArrayOf(ScalarType scalar) => new(scalar, true);

    public static readonly PirateType Int = Of(ScalarType.Int);
    public static readonly PirateType Float = Of(ScalarType.Float);
    public static readonly PirateType String = Of(ScalarType.String);
    public static readonly PirateType Char = Of(ScalarType.Char);
    public static readonly PirateType Bool = Of(ScalarType.Bool);
    public static readonly PirateType Void = Of(ScalarType.Void);

    /// <summary>
    /// Element type of an array type, or null when this is not an array.
    /// </summary>
    public PirateType? ElementType => IsArray ? new PirateType(Scalar, false) : null;

    public bool IsNumeric => !IsArray && Scalar is ScalarType.Int or ScalarType.Float;

    public override string ToString()
    {
        var name = Scalar switch
        {
            ScalarType.Int => "int",
            ScalarType.Float => "float",
            ScalarType.String => "string",
            ScalarType.Char => "char",
            ScalarType.Bool => "bool",
            ScalarType.Void => "void",
            _ => Scalar.ToString().ToLowerInvariant(),
        };
        return IsArray ? $"{name}[]" : name;
    }
}
