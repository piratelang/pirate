module Pirate.Lexer.Test.TokenTest

open Xunit
open Pirate.Lexer.Tokens
open Pirate.Lexer.Enums
open Pirate.Lexer.TokenType.Enums

[<Fact>]
let ShouldMatchToken () =
    Token(VALUE, INT, 123).Matches(TokenType.INT, 123) |> Assert.True
    
[<Fact>]
let ShouldNotMatchToken () =
    Token(VALUE, INT, 123).Matches(TokenType.INT, 456) |> Assert.False


