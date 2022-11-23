namespace PirateLexer.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum TokenTypeKeyword
{
    VAR,
    INT,
    FLOAT,
    STRING,
    CHAR,
    VOID,
    Empty
}