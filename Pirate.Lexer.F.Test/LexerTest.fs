module Pirate.Lexer.Test.LexerTest

open Xunit
open AutoFixture
open AutoFixture.Kernel
open Pirate.Common.FileHandler
open Pirate.Common.FileHandler.Interfaces
open Pirate.Common.Interfaces
open FakeItEasy
open Pirate.Common.Logger
open Pirate.Lexer
open Pirate.Lexer.TokenType.Enums

[<Fact>]
let ShouldMakeNumber () =
    // Arrange
    let logger = A.Fake<Logger>()
    let tokenRepository = A.Fake<TokenRepository>()

    // Act
    let result = Lexer(logger, tokenRepository).MakeTokens("123", "test")

    // Assert
    Assert.Single(result) |> ignore
    Assert.Equal(TokenGroup.VALUE, result.[0].TokenGroup)
    Assert.Equal(TokenType.INT, result.[0].TokenType)
    Assert.Equal(123, result.[0].Value |> unbox<int>)

[<Fact>]
let ShouldMakeFloat () =
    // Arrange
    let logger = A.Fake<Logger>()
    let tokenRepository = A.Fake<TokenRepository>()

    // Act
    let result = Lexer(logger, tokenRepository).MakeTokens("123.123", " ")

    // Assert
    Assert.Single(result) |> ignore
    Assert.Equal(TokenGroup.VALUE, result.[0].TokenGroup)
    Assert.Equal(TokenType.FLOAT, result.[0].TokenType)
    Assert.Equal(123.123, result.[0].Value |> unbox<float>)

[<Fact>]
let ShouldMakeIdentifier () = 
    // Arrange
    let logger = A.Fake<Logger>()
    let tokenRepository = A.Fake<TokenRepository>()

    // Act
    let result = Lexer(logger, tokenRepository).MakeTokens("appel", " ")

    // Assert
    Assert.Single(result) |> ignore
    Assert.Equal(TokenGroup.VALUE, result.[0].TokenGroup)
    Assert.Equal(TokenType.IDENTIFIER, result.[0].TokenType)
    Assert.Equal("appel", result.[0].Value |> unbox<string>)

[<Fact>]
let ShouldMakeString () = 
    // Arrange
    let logger = A.Fake<Logger>()
    let tokenRepository = A.Fake<TokenRepository>()

    // Act
    let result = Lexer(logger, tokenRepository).MakeTokens("\"test\"", " ")

    // Assert
    Assert.Single(result) |> ignore
    Assert.Equal(TokenGroup.VALUE, result.[0].TokenGroup)
    Assert.Equal(TokenType.STRING, result.[0].TokenType)
    Assert.Equal("test", result.[0].Value |> unbox<string>)

[<Fact>]
let ShouldMakeChar () = 
    // Arrange
    let logger = A.Fake<Logger>()
    let tokenRepository = A.Fake<TokenRepository>()

    // Act
    let result = Lexer(logger, tokenRepository).MakeTokens("'t'", " ")

    // Assert
    Assert.Single(result) |> ignore
    Assert.Equal(TokenGroup.VALUE, result.[0].TokenGroup)
    Assert.Equal(TokenType.CHAR, result.[0].TokenType)
    Assert.Equal("t", result.[0].Value |> unbox<string>)

[<Fact>]
let ShouldMakePlus () = 
    // Arrange
    let logger = A.Fake<Logger>()
    let tokenRepository = A.Fake<TokenRepository>()

    // Act
    let result = Lexer(logger, tokenRepository).MakeTokens("+", " ")

    // Assert
    Assert.Single(result) |> ignore
    Assert.Equal(TokenGroup.OPERATORS, result.[0].TokenGroup)
    Assert.Equal(TokenType.PLUS, result.[0].TokenType)
    Assert.Equal("+", result.[0].Value |> unbox<string>)

[<Fact>]
let ShouldMakeMinus () = 
    // Arrange
    let logger = A.Fake<Logger>()
    let tokenRepository = A.Fake<TokenRepository>()

    // Act
    let result = Lexer(logger, tokenRepository).MakeTokens("-", " ")

    // Assert
    Assert.Single(result) |> ignore
    Assert.Equal(TokenGroup.OPERATORS, result.[0].TokenGroup)
    Assert.Equal(TokenType.MINUS, result.[0].TokenType)
    Assert.Equal("-", result.[0].Value |> unbox<string>)

[<Fact>]
let ShouldMakeMultiply () = 
    // Arrange
    let logger = A.Fake<Logger>()
    let tokenRepository = A.Fake<TokenRepository>()

    // Act
    let result = Lexer(logger, tokenRepository).MakeTokens("*", " ")

    // Assert
    Assert.Single(result) |> ignore
    Assert.Equal(TokenGroup.OPERATORS, result.[0].TokenGroup)
    Assert.Equal(TokenType.MULTIPLY, result.[0].TokenType)
    Assert.Equal("*", result.[0].Value |> unbox<string>)

[<Fact>]
let ShouldMakeDivide () = 
    // Arrange
    let logger = A.Fake<Logger>()
    let tokenRepository = A.Fake<TokenRepository>()

    // Act
    let result = Lexer(logger, tokenRepository).MakeTokens("/", " ")

    // Assert
    Assert.Single(result) |> ignore
    Assert.Equal(TokenGroup.OPERATORS, result.[0].TokenGroup)
    Assert.Equal(TokenType.DIVIDE, result.[0].TokenType)
    Assert.Equal("/", result.[0].Value |> unbox<string>)

[<Fact>]
let ShouldMakePower () = 
    // Arrange
    let logger = A.Fake<Logger>()
    let tokenRepository = A.Fake<TokenRepository>()

    // Act
    let result = Lexer(logger, tokenRepository).MakeTokens("^", " ")

    // Assert
    Assert.Single(result) |> ignore
    Assert.Equal(TokenGroup.OPERATORS, result.[0].TokenGroup)
    Assert.Equal(TokenType.POWER, result.[0].TokenType)
    Assert.Equal("^", result.[0].Value |> unbox<string>)

[<Fact>]
let ShouldMakeLeftParentheses () = 
    // Arrange
    let logger = A.Fake<Logger>()
    let tokenRepository = A.Fake<TokenRepository>()

    // Act
    let result = Lexer(logger, tokenRepository).MakeTokens("(", " ")

    // Assert
    Assert.Single(result) |> ignore
    Assert.Equal(TokenGroup.SYNTAX, result.[0].TokenGroup)
    Assert.Equal(TokenType.LEFTPARENTHESES, result.[0].TokenType)
    Assert.Equal("(", result.[0].Value |> unbox<string>)

[<Fact>]
let ShouldMakeRightParentheses () = 
    // Arrange
    let logger = A.Fake<Logger>()
    let tokenRepository = A.Fake<TokenRepository>()

    // Act
    let result = Lexer(logger, tokenRepository).MakeTokens(")", " ")

    // Assert
    Assert.Single(result) |> ignore
    Assert.Equal(TokenGroup.SYNTAX, result.[0].TokenGroup)
    Assert.Equal(TokenType.RIGHTPARENTHESES, result.[0].TokenType)
    Assert.Equal(")", result.[0].Value |> unbox<string>)

[<Fact>]
let ShouldMakeLeftCurlyBrace () = 
    // Arrange
    let logger = A.Fake<Logger>()
    let tokenRepository = A.Fake<TokenRepository>()

    // Act
    let result = Lexer(logger, tokenRepository).MakeTokens("{", " ")

    // Assert
    Assert.Single(result) |> ignore
    Assert.Equal(TokenGroup.SYNTAX, result.[0].TokenGroup)
    Assert.Equal(TokenType.LEFTCURLYBRACE, result.[0].TokenType)
    Assert.Equal("{", result.[0].Value |> unbox<string>)

[<Fact>]
let ShouldMakeRightCurlyBrace () = 
    // Arrange
    let logger = A.Fake<Logger>()
    let tokenRepository = A.Fake<TokenRepository>()

    // Act
    let result = Lexer(logger, tokenRepository).MakeTokens("}", " ")

    // Assert
    Assert.Single(result) |> ignore
    Assert.Equal(TokenGroup.SYNTAX, result.[0].TokenGroup)
    Assert.Equal(TokenType.RIGHTCURLYBRACE, result.[0].TokenType)
    Assert.Equal("}", result.[0].Value |> unbox<string>)

[<Fact>]
let ShouldMakeComma () = 
    // Arrange
    let logger = A.Fake<Logger>()
    let tokenRepository = A.Fake<TokenRepository>()

    // Act
    let result = Lexer(logger, tokenRepository).MakeTokens(",", " ")

    // Assert
    Assert.Single(result) |> ignore
    Assert.Equal(TokenGroup.SYNTAX, result.[0].TokenGroup)
    Assert.Equal(TokenType.COMMA, result.[0].TokenType)
    Assert.Equal(",", result.[0].Value |> unbox<string>)

[<Fact>]
let ShouldMakeColon () = 
    // Arrange
    let logger = A.Fake<Logger>()
    let tokenRepository = A.Fake<TokenRepository>()

    // Act
    let result = Lexer(logger, tokenRepository).MakeTokens(":", " ")

    // Assert
    Assert.Single(result) |> ignore
    Assert.Equal(TokenGroup.SYNTAX, result.[0].TokenGroup)
    Assert.Equal(TokenType.COLON, result.[0].TokenType)
    Assert.Equal(":", result.[0].Value |> unbox<string>)

[<Fact>]
let ShouldMakeSemicolon () = 
    // Arrange
    let logger = A.Fake<Logger>()
    let tokenRepository = A.Fake<TokenRepository>()

    // Act
    let result = Lexer(logger, tokenRepository).MakeTokens(";", " ")

    // Assert
    Assert.Single(result) |> ignore
    Assert.Equal(TokenGroup.SYNTAX, result.[0].TokenGroup)
    Assert.Equal(TokenType.SEMICOLON, result.[0].TokenType)
    Assert.Equal(";", result.[0].Value |> unbox<string>)

[<Fact>]
let ShouldMakeDot () = 
    // Arrange
    let logger = A.Fake<Logger>()
    let tokenRepository = A.Fake<TokenRepository>()

    // Act
    let result = Lexer(logger, tokenRepository).MakeTokens(".", " ")

    // Assert
    Assert.Single(result) |> ignore
    Assert.Equal(TokenGroup.SYNTAX, result.[0].TokenGroup)
    Assert.Equal(TokenType.DOT, result.[0].TokenType)
    Assert.Equal(".", result.[0].Value |> unbox<string>)

[<Fact>]
let ShouldMakeLeftBracket () = 
    // Arrange
    let logger = A.Fake<Logger>()
    let tokenRepository = A.Fake<TokenRepository>()

    // Act
    let result = Lexer(logger, tokenRepository).MakeTokens("[", " ")

    // Assert
    Assert.Single(result) |> ignore
    Assert.Equal(TokenGroup.SYNTAX, result.[0].TokenGroup)
    Assert.Equal(TokenType.LEFTBRACKET, result.[0].TokenType)
    Assert.Equal("[", result.[0].Value |> unbox<string>)

[<Fact>]
let ShouldMakeRightBracket () = 
    // Arrange
    let logger = A.Fake<Logger>()
    let tokenRepository = A.Fake<TokenRepository>()

    // Act
    let result = Lexer(logger, tokenRepository).MakeTokens("]", " ")

    // Assert
    Assert.Single(result) |> ignore
    Assert.Equal(TokenGroup.SYNTAX, result.[0].TokenGroup)
    Assert.Equal(TokenType.RIGHTBRACKET, result.[0].TokenType)
    Assert.Equal("]", result.[0].Value |> unbox<string>)

[<Fact>]
let ShouldMakeEquals () = 
    // Arrange
    let logger = A.Fake<Logger>()
    let tokenRepository = A.Fake<TokenRepository>()

    // Act
    let result = Lexer(logger, tokenRepository).MakeTokens("=", " ")

    // Assert
    Assert.Single(result) |> ignore
    Assert.Equal(TokenGroup.SYNTAX, result.[0].TokenGroup)
    Assert.Equal(TokenType.EQUALS, result.[0].TokenType)
    Assert.Equal("=", result.[0].Value |> unbox<string>)

[<Fact>]
let ShouldMakeLessThan () = 
    // Arrange
    let logger = A.Fake<Logger>()
    let tokenRepository = A.Fake<TokenRepository>()

    // Act
    let result = Lexer(logger, tokenRepository).MakeTokens("<", " ")

    // Assert
    Assert.Single(result) |> ignore
    Assert.Equal(TokenGroup.COMPARISONOPERATORS, result.[0].TokenGroup)
    Assert.Equal(TokenType.LESSTHAN, result.[0].TokenType)
    Assert.Equal("<", result.[0].Value |> unbox<string>)

[<Fact>]
let ShouldMakeGreaterThan () = 
    // Arrange
    let logger = A.Fake<Logger>()
    let tokenRepository = A.Fake<TokenRepository>()

    // Act
    let result = Lexer(logger, tokenRepository).MakeTokens(">", " ")

    // Assert
    Assert.Single(result) |> ignore
    Assert.Equal(TokenGroup.COMPARISONOPERATORS, result.[0].TokenGroup)
    Assert.Equal(TokenType.GREATERTHAN, result.[0].TokenType)
    Assert.Equal(">", result.[0].Value |> unbox<string>)

[<Fact>]
let ShouldMakeNotEquals () = 
    // Arrange
    let logger = A.Fake<Logger>()
    let tokenRepository = A.Fake<TokenRepository>()

    // Act
    let result = Lexer(logger, tokenRepository).MakeTokens("!=", " ")

    // Assert
    Assert.Single(result) |> ignore
    Assert.Equal(TokenGroup.COMPARISONOPERATORS, result.[0].TokenGroup)
    Assert.Equal(TokenType.NOTEQUALS, result.[0].TokenType)
    Assert.Equal("!=", result.[0].Value |> unbox<string>)
