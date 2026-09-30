using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Pirate.Lexer.TokenType.Enums;

/// <summary>
/// A enum for token types.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum TokenType
{
    //Comparison Operators
    DOUBLEEQUALS,
    NOTEQUALS,
    LESSTHAN,
    GREATERTHAN,
    LESSTHANEQUALS,
    GREATERTHANEQUALS,
    DOUBLEPIPE,
    DOUBLEAMPERSAND,

    // Control Keywords
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
    EXTERN,

    // Operators
    PLUS,
    MINUS,
    MULTIPLY,
    DIVIDE,
    POWER,
    MODULO,

    // Syntax
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
    PLUSEQUALS,

    // Type Keywords & Values
    INT,
    FLOAT,
    STRING,
    CHAR,

    VAR,
    VOID,

    // Values
    // INT,
    // FLOAT,
    // STRING,
    // CHAR,

    Empty,

}
