module Pirate.Lexer.Test.TokenRepositoryTest

open Xunit
open Pirate.Lexer
open Pirate.Lexer.TokenType.Enums

[<Fact>]
let ShouldMakeNumber () =
    let tokenRepository = new TokenRepository(new KeyWordService())
    let text = "123"
    let position = 0
    
    let result = tokenRepository.MakeNumber(text, position)
    
    Assert.Equal(3, result.Position)
    Assert.Equal(TokenGroup.VALUE, result.Token.TokenGroup);
    Assert.Equal(TokenType.INT, result.Token.TokenType);
    Assert.Equal(123, result.Token.Value |> unbox<int>)
    
[<Fact>]
let ShouldMakeFloat () =
    let tokenRepository = new TokenRepository(new KeyWordService())
    let text = "123.456"
    let position = 0
    
    let result = tokenRepository.MakeNumber(text, position)

    Assert.Equal(7, result.Position)
    Assert.Equal(TokenGroup.VALUE, result.Token.TokenGroup)
    Assert.Equal(TokenType.FLOAT, result.Token.TokenType)
    Assert.Equal(123.456, result.Token.Value |> unbox<float>)
    
[<Fact>]
let ShouldMakeIdentifier () =
    let tokenRepository = new TokenRepository(new KeyWordService())
    let text = "abc"
    let position = 0
    
    let result = tokenRepository.MakeIdentifier(text, position)
    
    Assert.Equal(3, result.Position)
    Assert.Equal(TokenGroup.SYNTAX, result.Token.TokenGroup)
    Assert.Equal(TokenType.IDENTIFIER, result.Token.TokenType)
    Assert.Equal("abc", result.Token.Value |> unbox<string>)
    
[<Fact>]
let ShouldMakeKeyWord () =
    let tokenRepository = new TokenRepository(new KeyWordService())
    let text = "if"
    let position = 0
    
    let result = tokenRepository.MakeIdentifier(text, position)
    
    Assert.Equal(2, result.Position)
    Assert.Equal(TokenGroup.CONTROLKEYWORD, result.Token.TokenGroup)
    Assert.Equal(TokenType.IF, result.Token.TokenType)
    Assert.Equal("if", result.Token.Value |> unbox<string>)
    
[<Fact>]
let ShouldMakeString () =
    let tokenRepository = new TokenRepository(new KeyWordService())
    let text = "\"abc\""
    let position = 0
    
    let result = tokenRepository.MakeString(text, position)
    
    Assert.Equal(5, result.Position)
    Assert.Equal(TokenGroup.VALUE, result.Token.TokenGroup)
    Assert.Equal(TokenType.STRING, result.Token.TokenType)
    Assert.Equal("abc", result.Token.Value |> unbox<string>)
    

[<Fact>]
let ShouldMakeChar () =
    let tokenRepository = new TokenRepository(new KeyWordService())
    let text = "'a'"
    let position = 0
    
    let result = tokenRepository.MakeChar(text, position)
    
    Assert.Equal(3, result.Position)
    Assert.Equal(TokenGroup.VALUE, result.Token.TokenGroup)
    Assert.Equal(TokenType.CHAR, result.Token.TokenType)
    Assert.Equal("a", result.Token.Value |> unbox<string>)
    
[<Fact>]
let ShouldMakeNotEquals () =
    let tokenRepository = new TokenRepository(new KeyWordService())
    let text = "!="
    let position = 0
    
    let result = tokenRepository.MakeNotEquals(text, position)
    
    Assert.Equal(2, result.Position)
    Assert.Equal(TokenGroup.COMPARISONOPERATORS, result.Token.TokenGroup)
    Assert.Equal(TokenType.NOTEQUALS, result.Token.TokenType)
    Assert.Equal("!=", result.Token.Value |> unbox<string>)
    
[<Fact>]
let ShouldMakeGreaterThan  ()= 
    let tokenRepository = new TokenRepository(new KeyWordService())
    let text = ">"
    let position = 0
    
    let result = tokenRepository.MakeGreaterThan(text, position)
    
    Assert.Equal(1, result.Position)
    Assert.Equal(TokenGroup.COMPARISONOPERATORS, result.Token.TokenGroup)
    Assert.Equal(TokenType.GREATERTHAN, result.Token.TokenType)
    Assert.Equal(">", result.Token.Value |> unbox<string>)
    
[<Fact>]
let ShouldMakeGreaterThanEquals () = 
    let tokenRepository = new TokenRepository(new KeyWordService())
    let text = ">="
    let position = 0
    
    let result = tokenRepository.MakeGreaterThan(text, position)
    
    Assert.Equal(2, result.Position)
    Assert.Equal(TokenGroup.COMPARISONOPERATORS, result.Token.TokenGroup)
    Assert.Equal(TokenType.GREATERTHANEQUALS, result.Token.TokenType)
    Assert.Equal(">=", result.Token.Value |> unbox<string>)
    
[<Fact>]
let ShouldMakeLessThan () =
    let tokenRepository = new TokenRepository(new KeyWordService())
    let text = "<"
    let position = 0
    
    let result = tokenRepository.MakeLessThan(text, position)
    
    Assert.Equal(1, result.Position)
    Assert.Equal(TokenGroup.COMPARISONOPERATORS, result.Token.TokenGroup)
    Assert.Equal(TokenType.LESSTHAN, result.Token.TokenType)
    Assert.Equal("<", result.Token.Value |> unbox<string>)
    
[<Fact>]
let ShouldMakeLessThanEquals () =
    let tokenRepository = new TokenRepository(new KeyWordService())
    let text = "<="
    let position = 0
    
    let result = tokenRepository.MakeLessThan(text, position)
    
    Assert.Equal(2, result.Position)
    Assert.Equal(TokenGroup.COMPARISONOPERATORS, result.Token.TokenGroup)
    Assert.Equal(TokenType.LESSTHANEQUALS, result.Token.TokenType)
    Assert.Equal("<=", result.Token.Value |> unbox<string>)
    
[<Fact>]
let ShouldMakeEquals () =
    let tokenRepository = new TokenRepository(new KeyWordService())
    let text = "="
    let position = 0
    
    let result = tokenRepository.MakeEquals(text, position)
    
    Assert.Equal(1, result.Position)
    Assert.Equal(TokenGroup.SYNTAX, result.Token.TokenGroup)
    Assert.Equal(TokenType.EQUALS, result.Token.TokenType)
    Assert.Equal("=", result.Token.Value |> unbox<string>)
    
[<Fact>]
let ShouldMakeDoubleEquals () =
    let tokenRepository = new TokenRepository(new KeyWordService())
    let text = "=="
    let position = 0
    
    let result = tokenRepository.MakeEquals(text, position)
    
    Assert.Equal(2, result.Position)
    Assert.Equal(TokenGroup.COMPARISONOPERATORS, result.Token.TokenGroup)
    Assert.Equal(TokenType.EQUALS, result.Token.TokenType)
    Assert.Equal("==", result.Token.Value |> unbox<string>)
    
[<Fact>]
let ShouldMakePlus () =
    let tokenRepository = new TokenRepository(new KeyWordService())
    let text = "+"
    let position = 0
    
    let result = tokenRepository.MakePlus(text, position)
    
    Assert.Equal(1, result.Position)
    Assert.Equal(TokenGroup.OPERATORS, result.Token.TokenGroup)
    Assert.Equal(TokenType.PLUS, result.Token.TokenType)
    Assert.Equal("+", result.Token.Value |> unbox<string>)
    
[<Fact>]
let ShouldMakePlusEquals () =
    let tokenRepository = new TokenRepository(new KeyWordService())
    let text = "+="
    let position = 0
    
    let result = tokenRepository.MakePlus(text, position)
    
    Assert.Equal(2, result.Position)
    Assert.Equal(TokenGroup.SYNTAX, result.Token.TokenGroup)
    Assert.Equal(TokenType.PLUSEQUALS, result.Token.TokenType)
    Assert.Equal("+=", result.Token.Value |> unbox<string>)
    
[<Fact>]
let ShouldMakeDivide () =
    let tokenRepository = new TokenRepository(new KeyWordService())
    let text = "/"
    let position = 0
    
    let result = tokenRepository.MakeDivide(text, position)
    
    Assert.Equal(1, result.Position)
    Assert.Equal(TokenGroup.OPERATORS, result.Token.TokenGroup)
    Assert.Equal(TokenType.DIVIDE, result.Token.TokenType)
    Assert.Equal("/", result.Token.Value |> unbox<string>)
    
[<Fact>]
let ShouldMakeDoubleDivide () =
    let tokenRepository = new TokenRepository(new KeyWordService())
    let text = "//"
    let position = 0
    
    let result = tokenRepository.MakeDivide(text, position)
    
    Assert.Equal(2, result.Position)
    Assert.Equal(TokenGroup.SYNTAX, result.Token.TokenGroup)
    Assert.Equal(TokenType.DOUBLEDIVIDE, result.Token.TokenType)
    Assert.Equal("//", result.Token.Value |> unbox<string>)