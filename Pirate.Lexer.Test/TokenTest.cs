using Pirate.Lexer.Enums;
using Pirate.Lexer.Tokens;

namespace Pirate.Lexer.Test;

public class TokenTest
{
    [Fact]
    public void ShouldMatchToken()
    {
        //Arrange
        var token = new Token(TokenGroup.VALUE, TokenType.INT, 123);
        //Act
        var result = token.Matches(TokenType.INT, 123);
        //Assert
        Assert.True(result);
    }

    [Fact]
    public void ShouldNotMatchToken()
    {
        //Arrange
        var token = new Token(TokenGroup.VALUE, TokenType.INT, 123);
        //Act
        var result = token.Matches(TokenType.INT, 456);
        //Assert
        Assert.False(result);
    }

    [Fact]
    public void ShouldMatchTokenWithoutValue()
    {
        //Arrange
        var token = new Token(TokenGroup.VALUE, TokenType.INT, 123);
        //Act
        var result = token.Matches(TokenType.INT);
        //Assert
        Assert.True(result);
    }

    [Fact]
    public void ShouldNotMatchTokenWithoutValue()
    {
        //Arrange
        var token = new Token(TokenGroup.VALUE, TokenType.INT, 123);
        //Act
        var result = token.Matches(TokenType.FLOAT);
        //Assert
        Assert.False(result);
    }
}