

namespace PirateLexer.Test;


public class TokenRepositoryTest
{
    [Fact]
    public void ShouldMakeNumber()
    {
        //Arrange
        var tokenRepository = new TokenRepository(new KeyWordService());
        var text = "123";
        var position = 0;
        //Act
        var result = tokenRepository.MakeNumber(text, position);
        //Assert
        Assert.Equal(3, result.Position);
        Assert.Equal(TokenGroup.VALUE, result.Token.TokenGroup);
        Assert.Equal(TokenValue.INT, result.Token.TokenType);
        Assert.Equal(123, result.Token.Value);
    }

    [Fact]
    public void ShouldMakeFloat()
    {
        //Arrange
        var tokenRepository = new TokenRepository(new KeyWordService());
        var text = "123.123";
        var position = 0;
        //Act
        var result = tokenRepository.MakeNumber(text, position);
        //Assert
        Assert.Equal(7, result.Position);
        Assert.Equal(TokenGroup.VALUE, result.Token.TokenGroup);
        Assert.Equal(TokenValue.FLOAT, result.Token.TokenType);
        Assert.Equal((float)123.123, result.Token.Value);
    }

    [Fact]
    public void ShouldMakeIdentifier()
    {
        //Arrange
        var tokenRepository = new TokenRepository(new KeyWordService());
        var text = "test";
        var position = 0;
        //Act
        var result = tokenRepository.MakeIdentifier(text, position);
        //Assert
        Assert.Equal(4, result.Position);
        Assert.Equal(TokenGroup.SYNTAX, result.Token.TokenGroup);
        Assert.Equal(TokenSyntax.IDENTIFIER, result.Token.TokenType);
        Assert.Equal("test", result.Token.Value);
    }

    [Fact]
    public void ShouldMakeKeyWord()
    {
        //Arrange
        var tokenRepository = new TokenRepository(new KeyWordService());
        var text = "if";
        var position = 0;
        //Act
        var result = tokenRepository.MakeIdentifier(text, position);
        //Assert
        Assert.Equal(2, result.Position);
        Assert.Equal(TokenGroup.CONTROLKEYWORD, result.Token.TokenGroup);
        Assert.Equal(TokenControlKeyword.IF, result.Token.TokenType);
        Assert.Equal("if", result.Token.Value);
    }

    [Fact]
    public void ShouldMakeString()
    {
        //Arrange
        var tokenRepository = new TokenRepository(new KeyWordService());
        var text = "\"test\"";
        var position = 0;
        //Act
        var result = tokenRepository.MakeString(text, position);
        //Assert
        Assert.Equal(6, result.Position);
        Assert.Equal(TokenGroup.VALUE, result.Token.TokenGroup);
        Assert.Equal(TokenValue.STRING, result.Token.TokenType);
        Assert.Equal("test", result.Token.Value);
    }

    [Fact]
    public void ShouldMakeChar()
    {
        //Arrange
        var tokenRepository = new TokenRepository(new KeyWordService());
        var text = "'a'";
        var position = 0;
        //Act
        var result = tokenRepository.MakeChar(text, position);
        //Assert
        Assert.Equal(3, result.Position);
        Assert.Equal(TokenGroup.VALUE, result.Token.TokenGroup);
        Assert.Equal(TokenValue.CHAR, result.Token.TokenType);
        Assert.Equal('a', result.Token.Value);
    }

    [Fact]
    public void ShouldMakeNotEquals()
    {
        //Arrange
        var tokenRepository = new TokenRepository(new KeyWordService());
        var text = "!=";
        var position = 0;
        //Act
        var result = tokenRepository.MakeNotEquals(text, position);
        //Assert
        Assert.Equal(2, result.Position);
        Assert.Equal(TokenGroup.COMPARISONOPERATORS, result.Token.TokenGroup);
        Assert.Equal(TokenComparisonOperators.NOTEQUALS, result.Token.TokenType);
    }

    [Fact]
    public void ShouldMakeGreaterThan()
    {
        //Arrange
        var tokenRepository = new TokenRepository(new KeyWordService());
        var text = "> ";
        var position = 0;
        //Act
        var result = tokenRepository.MakeGreaterThan(text, position);
        //Assert
        Assert.Equal(1, result.Position);
        Assert.Equal(TokenGroup.COMPARISONOPERATORS, result.Token.TokenGroup);
        Assert.Equal(TokenComparisonOperators.GREATERTHAN, result.Token.TokenType);
    }

    [Fact]
    public void ShouldMakeGreaterThanEquals()
    {
        //Arrange
        var tokenRepository = new TokenRepository(new KeyWordService());
        var text = ">=";
        var position = 0;
        //Act
        var result = tokenRepository.MakeGreaterThan(text, position);
        //Assert
        Assert.Equal(2, result.Position);
        Assert.Equal(TokenGroup.COMPARISONOPERATORS, result.Token.TokenGroup);
        Assert.Equal(TokenComparisonOperators.GREATERTHANEQUALS, result.Token.TokenType);
    }

    [Fact]
    public void ShouldMakeLessThan()
    {
        //Arrange
        var tokenRepository = new TokenRepository(new KeyWordService());
        var text = "< ";
        var position = 0;
        //Act
        var result = tokenRepository.MakeLessThan(text, position);
        //Assert
        Assert.Equal(1, result.Position);
        Assert.Equal(TokenGroup.COMPARISONOPERATORS, result.Token.TokenGroup);
        Assert.Equal(TokenComparisonOperators.LESSTHAN, result.Token.TokenType);
    }

    [Fact]
    public void ShouldMakeLessThanEquals()
    {
        //Arrange
        var tokenRepository = new TokenRepository(new KeyWordService());
        var text = "<=";
        var position = 0;
        //Act
        var result = tokenRepository.MakeLessThan(text, position);
        //Assert
        Assert.Equal(2, result.Position);
        Assert.Equal(TokenGroup.COMPARISONOPERATORS, result.Token.TokenGroup);
        Assert.Equal(TokenComparisonOperators.LESSTHANEQUALS, result.Token.TokenType);
    }

    [Fact]
    public void ShouldMakeEquals()
    {
        //Arrange
        var tokenRepository = new TokenRepository(new KeyWordService());
        var text = "= ";
        var position = 0;
        //Act
        var result = tokenRepository.MakeEquals(text, position);
        //Assert
        Assert.Equal(1, result.Position);
        Assert.Equal(TokenGroup.SYNTAX, result.Token.TokenGroup);
        Assert.Equal(TokenSyntax.EQUALS, result.Token.TokenType);
    }

    [Fact]
    public void ShouldMakeDoubleEquals()
    {
        //Arrange
        var tokenRepository = new TokenRepository(new KeyWordService());
        var text = "==";
        var position = 0;
        //Act
        var result = tokenRepository.MakeEquals(text, position);
        //Assert
        Assert.Equal(2, result.Position);
        Assert.Equal(TokenGroup.COMPARISONOPERATORS, result.Token.TokenGroup);
        Assert.Equal(TokenComparisonOperators.DOUBLEEQUALS, result.Token.TokenType);
    }

    [Fact]
    public void ShouldMakePlus()
    {
        //Arrange
        var tokenRepository = new TokenRepository(new KeyWordService());
        var text = "+ ";
        var position = 0;
        //Act
        var result = tokenRepository.MakePlus(text, position);
        //Assert
        Assert.Equal(1, result.Position);
        Assert.Equal(TokenGroup.OPERATORS, result.Token.TokenGroup);
        Assert.Equal(TokenOperators.PLUS, result.Token.TokenType);
    }

    [Fact]
    public void ShouldMakePlusEquals()
    {
        //Arrange
        var tokenRepository = new TokenRepository(new KeyWordService());
        var text = "+=";
        var position = 0;
        //Act
        var result = tokenRepository.MakePlus(text, position);
        //Assert
        Assert.Equal(2, result.Position);
        Assert.Equal(TokenGroup.SYNTAX, result.Token.TokenGroup);
        Assert.Equal(TokenSyntax.PLUSEQUALS, result.Token.TokenType);
    }

    [Fact]
    public void ShouldMakeDivide()
    {
        //Arrange
        var tokenRepository = new TokenRepository(new KeyWordService());
        var text = "/ ";
        var position = 0;
        //Act
        var result = tokenRepository.MakeDivide(text, position);
        //Assert
        Assert.Equal(1, result.Position);
        Assert.Equal(TokenGroup.OPERATORS, result.Token.TokenGroup);
        Assert.Equal(TokenOperators.DIVIDE, result.Token.TokenType);
    }

    [Fact]
    public void ShouldMakeDoubleDivide()
    {
        //Arrange
        var tokenRepository = new TokenRepository(new KeyWordService());
        var text = "//";
        var position = 0;
        //Act
        var result = tokenRepository.MakeDivide(text, position);
        //Assert
        Assert.Equal(2, result.Position);
        Assert.Equal(TokenGroup.SYNTAX, result.Token.TokenGroup);
        Assert.Equal(TokenSyntax.DOUBLEDIVIDE, result.Token.TokenType);
    }
}
