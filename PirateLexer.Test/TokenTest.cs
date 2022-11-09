namespace PirateLexer.Test;

public class TokenTest
{
    [Fact]
    public void ShouldMatchToken()
    {
        //Arrange
        var token = new Token(TokenGroup.VALUE, TokenValue.INT, 123);
        //Act
        var result = token.Matches(TokenValue.INT, 123);
        //Assert
        Assert.True(result);
    }

    [Fact]
    public void ShouldNotMatchToken()
    {
        //Arrange
        var token = new Token(TokenGroup.VALUE, TokenValue.INT, 123);
        //Act
        var result = token.Matches(TokenValue.INT, 456);
        //Assert
        Assert.False(result);
    }

    [Fact]
    public void ShouldMatchTokenWithoutValue()
    {
        //Arrange
        var token = new Token(TokenGroup.VALUE, TokenValue.INT, 123);
        //Act
        var result = token.Matches(TokenValue.INT);
        //Assert
        Assert.True(result);
    }

    [Fact]
    public void ShouldNotMatchTokenWithoutValue()
    {
        //Arrange
        var token = new Token(TokenGroup.VALUE, TokenValue.INT, 123);
        //Act
        var result = token.Matches(TokenValue.FLOAT);
        //Assert
        Assert.False(result);
    }
}