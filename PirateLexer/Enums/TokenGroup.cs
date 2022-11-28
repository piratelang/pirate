namespace PirateLexer.Enums;

/// <summary>
/// A enum for token groups.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum TokenGroup
{
    OPERATORS,
    COMPARISONOPERATORS,
    TYPEKEYWORD,
    CONTROLKEYWORD,
    SYNTAX,
    VALUE,
    Empty
}
