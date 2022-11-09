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
}