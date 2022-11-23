namespace PirateLexer.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum TokenValue
{
    INT,
    FLOAT,
    STRING,
    CHAR
}
