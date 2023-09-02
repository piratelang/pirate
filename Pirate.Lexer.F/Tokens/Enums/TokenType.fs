namespace Pirate.Lexer.F.Tokens.Enums

open Newtonsoft.Json
open Newtonsoft.Json.Converters

[<JsonConverter(typeof<StringEnumConverter>)>]
type TokenType =
    // Comparison Operators
    | DOUBLEEQUALS
    | NOTEQUALS
    | LESSTHAN
    | GREATERTHAN
    | LESSTHANEQUALS
    | GREATERTHANEQUALS
    | DOUBLEPIPE
    | DOUBLEAMPERSAND

    // Control Keywords
    | IF
    | ELSE
    | FOR
    | TO
    | FOREACH
    | IN
    | WHILE
    | FUNC
    | CLASS
    | NEW
    | RETURN

    // Operators
    | PLUS
    | MINUS
    | MULTIPLY
    | DIVIDE
    | POWER
    | MODULO

    // Syntax
    | IDENTIFIER
    | LEFTPARENTHESES
    | RIGHTPARENTHESES
    | RIGHTCURLYBRACE
    | LEFTCURLYBRACE
    | LEFTBRACKET
    | RIGHTBRACKET
    | COMMA
    | COLON
    | SEMICOLON
    | DOT
    | DOLLAR
    | DOUBLEDIVIDE
    | EQUALS
    | PLUSEQUALS

    // Type Keywords and Values
    | INT
    | FLOAT
    | STRING
    | CHAR
    | VAR
    | VOID

    | Empty
