namespace PirateLexer.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum TokenControlKeyword
{
    IF,
    ELSE,
    FOR,
    TO,
    FOREACH,
    IN,
    WHILE,
    FUNC,
    CLASS,
    NEW,
    RETURN,
    Empty
}
