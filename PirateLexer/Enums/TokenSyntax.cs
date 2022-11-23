namespace PirateLexer.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum TokenSyntax
{
    IDENTIFIER,
    LEFTPARENTHESES,
    RIGHTPARENTHESES,
    RIGHTCURLYBRACE,
    LEFTCURLYBRACE,
    LEFTBRACKET,
    RIGHTBRACKET,
    COMMA,
    COLON,
    SEMICOLON,
    DOT,
    DOLLAR,
    DOUBLEDIVIDE,
    EQUALS,
    PLUSEQUALS
}