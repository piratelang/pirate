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
    
[<Fact>]
let ShouldMakeIdentifier () =
    let tokenRepository = new TokenRepository(new KeyWordService())
    let text = "abc"
    let position = 0
    
    let result = tokenRepository.MakeIdentifier(text, position)
    
    Assert.Equal(3, result.Position)
    Assert.Equal(SYNTAX, result.Token.TokenGroup)
    Assert.Equal(IDENTIFIER, result.Token.TokenType)
    Assert.Equal("abc", result.Token.Value |> unbox<string>)
    
[<Fact>]
let ShouldMakeKeyWord () =
    let tokenRepository = new TokenRepository(new KeyWordService())
    let text = "if"
    let position = 0
    
    let result = tokenRepository.MakeIdentifier(text, position)
    
    Assert.Equal(2, result.Position)
    Assert.Equal(CONTROLKEYWORD, result.Token.TokenGroup)
    Assert.Equal(IF, result.Token.TokenType)
    Assert.Equal("if", result.Token.Value |> unbox<string>)
    
[<Fact>]
let ShouldMakeString () =
    let tokenRepository = new TokenRepository(new KeyWordService())
    let text = "\"abc\""
    let position = 0
    
    let result = tokenRepository.MakeString(text, position)
    
    Assert.Equal(5, result.Position)
    Assert.Equal(VALUE, result.Token.TokenGroup)
    Assert.Equal(STRING, result.Token.TokenType)
    Assert.Equal("abc", result.Token.Value |> unbox<string>)
    

[<Fact>]
let ShouldMakeChar () =
    let tokenRepository = new TokenRepository(new KeyWordService())
    let text = "'a'"
    let position = 0
    
    let result = tokenRepository.MakeChar(text, position)
    
    Assert.Equal(3, result.Position)
    Assert.Equal(VALUE, result.Token.TokenGroup)
    Assert.Equal(CHAR, result.Token.TokenType)
    Assert.Equal("a", result.Token.Value |> unbox<string>)
    
[<Fact>]
let ShouldMakeNotEquals () =
    let tokenRepository = new TokenRepository(new KeyWordService())
    let text = "!="
    let position = 0
    
    let result = tokenRepository.MakeNotEquals(text, position)
    
    Assert.Equal(2, result.Position)
    Assert.Equal(COMPARISONOPERATORS, result.Token.TokenGroup)
    Assert.Equal(NOTEQUALS, result.Token.TokenType)
    Assert.Equal("!=", result.Token.Value |> unbox<string>)
    
[<Fact>]
let ShouldMakeGreaterThan  ()= 
    let tokenRepository = new TokenRepository(new KeyWordService())
    let text = ">"
    let position = 0
    
    let result = tokenRepository.MakeGreaterThan(text, position)
    
    Assert.Equal(1, result.Position)
    Assert.Equal(COMPARISONOPERATORS, result.Token.TokenGroup)
    Assert.Equal(GREATERTHAN, result.Token.TokenType)
    Assert.Equal(">", result.Token.Value |> unbox<string>)
    
[<Fact>]
let ShouldMakeGreaterThanEquals () = 
    let tokenRepository = new TokenRepository(new KeyWordService())
    let text = ">="
    let position = 0
    
    let result = tokenRepository.MakeGreaterThan(text, position)
    
    Assert.Equal(2, result.Position)
    Assert.Equal(COMPARISONOPERATORS, result.Token.TokenGroup)
    Assert.Equal(GREATERTHANEQUALS, result.Token.TokenType)
    Assert.Equal(">=", result.Token.Value |> unbox<string>)
    
[<Fact>]
let ShouldMakeLessThan () =
    let tokenRepository = new TokenRepository(new KeyWordService())
    let text = "<"
    let position = 0
    
    let result = tokenRepository.MakeLessThan(text, position)
    
    Assert.Equal(1, result.Position)
    Assert.Equal(COMPARISONOPERATORS, result.Token.TokenGroup)
    Assert.Equal(LESSTHAN, result.Token.TokenType)
    Assert.Equal("<", result.Token.Value |> unbox<string>)
    
[<Fact>]
let ShouldMakeLessThanEquals () =
    let tokenRepository = new TokenRepository(new KeyWordService())
    let text = "<="
    let position = 0
    
    let result = tokenRepository.MakeLessThan(text, position)
    
    Assert.Equal(2, result.Position)
    Assert.Equal(COMPARISONOPERATORS, result.Token.TokenGroup)
    Assert.Equal(LESSTHANEQUALS, result.Token.TokenType)
    Assert.Equal("<=", result.Token.Value |> unbox<string>)
    
[<Fact>]
let ShouldMakeEquals () =
    let tokenRepository = new TokenRepository(new KeyWordService())
    let text = "="
    let position = 0
    
    let result = tokenRepository.MakeEquals(text, position)
    
    Assert.Equal(1, result.Position)
    Assert.Equal(SYNTAX, result.Token.TokenGroup)
    Assert.Equal(EQUALS, result.Token.TokenType)
    Assert.Equal("=", result.Token.Value |> unbox<string>)
    
[<Fact>]
let ShouldMakeDoubleEquals () =
    let tokenRepository = new TokenRepository(new KeyWordService())
    let text = "=="
    let position = 0
    
    let result = tokenRepository.MakeEquals(text, position)
    
    Assert.Equal(2, result.Position)
    Assert.Equal(COMPARISONOPERATORS, result.Token.TokenGroup)
    Assert.Equal(EQUALS, result.Token.TokenType)
    Assert.Equal("==", result.Token.Value |> unbox<string>)
    
[<Fact>]
let ShouldMakePlus () =
    let tokenRepository = new TokenRepository(new KeyWordService())
    let text = "+"
    let position = 0
    
    let result = tokenRepository.MakePlus(text, position)
    
    Assert.Equal(1, result.Position)
    Assert.Equal(OPERATORS, result.Token.TokenGroup)
    Assert.Equal(PLUS, result.Token.TokenType)
    Assert.Equal("+", result.Token.Value |> unbox<string>)
    
[<Fact>]
let ShouldMakePlusEquals () =
    let tokenRepository = new TokenRepository(new KeyWordService())
    let text = "+="
    let position = 0
    
    let result = tokenRepository.MakePlus(text, position)
    
    Assert.Equal(2, result.Position)
    Assert.Equal(SYNTAX, result.Token.TokenGroup)
    Assert.Equal(PLUSEQUALS, result.Token.TokenType)
    Assert.Equal("+=", result.Token.Value |> unbox<string>)
    
[<Fact>]
let ShouldMakeDivide () =
    let tokenRepository = new TokenRepository(new KeyWordService())
    let text = "/"
    let position = 0
    
    let result = tokenRepository.MakeDivide(text, position)
    
    Assert.Equal(1, result.Position)
    Assert.Equal(OPERATORS, result.Token.TokenGroup)
    Assert.Equal(DIVIDE, result.Token.TokenType)
    Assert.Equal("/", result.Token.Value |> unbox<string>)
    
[<Fact>]
let ShouldMakeDoubleDivide () =
    let tokenRepository = new TokenRepository(new KeyWordService())
    let text = "//"
    let position = 0
    
    let result = tokenRepository.MakeDivide(text, position)
    
    Assert.Equal(2, result.Position)
    Assert.Equal(SYNTAX, result.Token.TokenGroup)
    Assert.Equal(DOUBLEDIVIDE, result.Token.TokenType)
    Assert.Equal("//", result.Token.Value |> unbox<string>)