using Pirate.Lexer.Enums;
using Pirate.Lexer;

namespace Pirate.Lexer.Test;


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
        Assert.Equal(TokenType.INT, result.Token.TokenType);
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
        Assert.Equal(TokenType.FLOAT, result.Token.TokenType);
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
        Assert.Equal(TokenType.IDENTIFIER, result.Token.TokenType);
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
        Assert.Equal(TokenType.IF, result.Token.TokenType);
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
        Assert.Equal(TokenType.STRING, result.Token.TokenType);
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
        Assert.Equal(TokenType.CHAR, result.Token.TokenType);
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
        Assert.Equal(TokenType.NOTEQUALS, result.Token.TokenType);
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
        Assert.Equal(TokenType.GREATERTHAN, result.Token.TokenType);
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
        Assert.Equal(TokenType.GREATERTHANEQUALS, result.Token.TokenType);
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
        Assert.Equal(TokenType.LESSTHAN, result.Token.TokenType);
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
        Assert.Equal(TokenType.LESSTHANEQUALS, result.Token.TokenType);
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
        Assert.Equal(TokenType.EQUALS, result.Token.TokenType);
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
        Assert.Equal(TokenType.DOUBLEEQUALS, result.Token.TokenType);
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
        Assert.Equal(TokenType.PLUS, result.Token.TokenType);
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
        Assert.Equal(TokenType.PLUSEQUALS, result.Token.TokenType);
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
        Assert.Equal(TokenType.DIVIDE, result.Token.TokenType);
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
        Assert.Equal(TokenType.DOUBLEDIVIDE, result.Token.TokenType);
    }
}
