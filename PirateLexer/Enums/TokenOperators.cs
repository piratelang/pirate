namespace PirateLexer.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum TokenOperators
{
    PLUS,
    MINUS,
    MULTIPLY,
    DIVIDE,
    POWER
}
