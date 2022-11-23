namespace PirateLexer.Enums;

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

    // Operators
    PLUS,
    MINUS,
    MULTIPLY,
    DIVIDE,
    POWER,

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