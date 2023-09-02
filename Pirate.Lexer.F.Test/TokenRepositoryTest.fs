module Pirate.Lexer.F.Test.TokenRepositoryTest

open Xunit
open Pirate.Lexer.F
open Pirate.Lexer.F.Tokens.Enums

[<Fact>]
let ShouldMakeNumber () =
    let tokenRepository = new TokenRepository(new KeyWordService())
    let text = "123"
    let position = 0
    
    let result = tokenRepository.MakeNumber(text, position)
    
    Assert.Equal(3, result.Position)
    Assert.Equal(VALUE, result.Token.TokenGroup);
    Assert.Equal(INT, result.Token.TokenType);
    Assert.Equal(123, result.Token.Value |> unbox<int>)
    
[<Fact>]
let ShouldMakeFloat () =
    let tokenRepository = new TokenRepository(new KeyWordService())
    let text = "123.456"
    let position = 0
    
    let result = tokenRepository.MakeNumber(text, position)
    
    Assert.Equal(7, result.Position)
    Assert.Equal(VALUE, result.Token.TokenGroup)
    Assert.Equal(FLOAT, result.Token.TokenType)
    Assert.Equal(123.456, result.Token.Value |> unbox<float>)