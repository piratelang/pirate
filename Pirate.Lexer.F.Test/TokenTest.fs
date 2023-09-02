module Pirate.Lexer.F.Test.TokenTest

open Xunit
open Pirate.Lexer.F.Tokens
open Pirate.Lexer.F.Tokens.Enums

[<Fact>]
let ShouldMatchToken () =
    Token(VALUE, INT, 123).Matches(TokenType.INT, 123) |> Assert.True
    
[<Fact>]
let ShouldNotMatchToken () =
    Token(VALUE, INT, 123).Matches(TokenType.INT, 456) |> Assert.False


