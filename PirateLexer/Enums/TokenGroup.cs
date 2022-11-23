namespace PirateLexer.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum TokenGroup
{
    OPERATORS,
    COMPARISONOPERATORS,
    TYPEKEYWORD,
    CONTROLKEYWORD,
    SYNTAX,
    VALUE
}
