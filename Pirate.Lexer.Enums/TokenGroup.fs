namespace Pirate.Lexer.Enums

open Newtonsoft.Json
open Newtonsoft.Json.Converters

[<JsonConverter(typeof<StringEnumConverter>)>]
type TokenGroup =
    | OPERATORS
    | COMPARISONOPERATORS
    | TYPEKEYWORD
    | CONTROLKEYWORD
    | SYNTAX
    | VALUE
    | Empty