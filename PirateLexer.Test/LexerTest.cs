
using Pirate.Common;
using Pirate.Common.FileHandlers;
using Pirate.Common.FileHandlers.Interfaces;
using Pirate.Common.Interfaces;
using Pirate.Lexer.Enums;
using Pirate.Lexer.Tokens;

namespace Pirate.Lexer.Test;

public class LexerTest
{
    [Fact]
    public void ShouldMakeNumber()
    {
        //Arrange
        Fixture fixture = new();
        fixture.Customizations.Add(new TypeRelay(typeof(IFileWriteHandler), typeof(FileWriteHandler)));
        fixture.Customizations.Add(new TypeRelay(typeof(IEnvironmentVariables), typeof(EnvironmentVariables)));

        var environmentVariables = A.Fake<IEnvironmentVariables>();
        A.CallTo(() => environmentVariables.GetVariable("version")).Returns("1.0.0");
        var logger = A.Fake<Logger>();
        var tokenRepository = A.Fake<TokenRepository>();

        Lexer lexer = new(logger, tokenRepository);

        //Act
        var result = lexer.MakeTokens("123", " ");

        //Assert
        Assert.Single(result);
        Assert.Equal(TokenGroup.VALUE, result[0].TokenGroup);
        Assert.Equal(TokenType.INT, result[0].TokenType);
        Assert.Equal(123, result[0].Value);
    }

    [Fact]
    public void ShouldMakeFloat()
    {
        //Arrange
        Fixture fixture = new();
        fixture.Customizations.Add(new TypeRelay(typeof(IFileWriteHandler), typeof(FileWriteHandler)));
        fixture.Customizations.Add(new TypeRelay(typeof(IEnvironmentVariables), typeof(EnvironmentVariables)));

        var environmentVariables = A.Fake<IEnvironmentVariables>();
        A.CallTo(() => environmentVariables.GetVariable("version")).Returns("1.0.0");
        var logger = A.Fake<Logger>();
        var tokenRepository = A.Fake<TokenRepository>();

        Lexer lexer = new(logger, tokenRepository);

        //Act
        var result = lexer.MakeTokens("123.123", " ");

        //Assert
        Assert.Single(result);
        Assert.Equal(TokenGroup.VALUE, result[0].TokenGroup);
        Assert.Equal(TokenType.FLOAT, result[0].TokenType);
        Assert.Equal((float)123.123, result[0].Value);
    }

    [Fact]
    public void ShouldMakeIdentifier()
    {
        //Arrange
        Fixture fixture = new();
        fixture.Customizations.Add(new TypeRelay(typeof(IFileWriteHandler), typeof(FileWriteHandler)));
        fixture.Customizations.Add(new TypeRelay(typeof(IEnvironmentVariables), typeof(EnvironmentVariables)));

        var environmentVariables = A.Fake<IEnvironmentVariables>();
        A.CallTo(() => environmentVariables.GetVariable("version")).Returns("1.0.0");
        var logger = A.Fake<Logger>();
        var tokenRepository = new TokenRepository(new KeyWordService());

        Lexer lexer = new(logger, tokenRepository);

        //Act
        var result = lexer.MakeTokens("appel", " ");

        //Assert
        Assert.Single(result);
        Assert.Equal(TokenGroup.SYNTAX, result[0].TokenGroup);
        Assert.Equal(TokenType.IDENTIFIER, result[0].TokenType);
        Assert.Equal("appel", result[0].Value);
    }

    [Fact]
    public void ShouldMakeString()
    {
        //Arrange
        Fixture fixture = new();
        fixture.Customizations.Add(new TypeRelay(typeof(IFileWriteHandler), typeof(FileWriteHandler)));
        fixture.Customizations.Add(new TypeRelay(typeof(IEnvironmentVariables), typeof(EnvironmentVariables)));

        var environmentVariables = A.Fake<IEnvironmentVariables>();
        A.CallTo(() => environmentVariables.GetVariable("version")).Returns("1.0.0");
        var logger = A.Fake<Logger>();
        var tokenRepository = A.Fake<TokenRepository>();

        Lexer lexer = new(logger, tokenRepository);

        //Act
        var result = lexer.MakeTokens("\"test\"", " ");

        //Assert
        Assert.Single(result);
        Assert.Equal(TokenGroup.VALUE, result[0].TokenGroup);
        Assert.Equal(TokenType.STRING, result[0].TokenType);
        Assert.Equal("test", result[0].Value);
    }

    [Fact]
    public void ShouldMakeChar()
    {
        //Arrange
        Fixture fixture = new();
        fixture.Customizations.Add(new TypeRelay(typeof(IFileWriteHandler), typeof(FileWriteHandler)));
        fixture.Customizations.Add(new TypeRelay(typeof(IEnvironmentVariables), typeof(EnvironmentVariables)));

        var environmentVariables = A.Fake<IEnvironmentVariables>();
        A.CallTo(() => environmentVariables.GetVariable("version")).Returns("1.0.0");
        var logger = A.Fake<Logger>();
        var tokenRepository = A.Fake<TokenRepository>();

        Lexer lexer = new(logger, tokenRepository);

        //Act
        var result = lexer.MakeTokens("'a'", " ");

        //Assert
        Assert.Single(result);
        Assert.Equal(TokenGroup.VALUE, result[0].TokenGroup);
        Assert.Equal(TokenType.CHAR, result[0].TokenType);
        Assert.Equal('a', result[0].Value);
    }

    [Fact]
    public void ShouldMakePlus()
    {
        //Arrange
        Fixture fixture = new();
        fixture.Customizations.Add(new TypeRelay(typeof(IFileWriteHandler), typeof(FileWriteHandler)));
        fixture.Customizations.Add(new TypeRelay(typeof(IEnvironmentVariables), typeof(EnvironmentVariables)));

        var environmentVariables = A.Fake<IEnvironmentVariables>();
        A.CallTo(() => environmentVariables.GetVariable("version")).Returns("1.0.0");
        var logger = A.Fake<Logger>();
        var tokenRepository = A.Fake<TokenRepository>();

        Lexer lexer = new(logger, tokenRepository);

        //Act
        var result = lexer.MakeTokens("+ ", " ");

        //Assert
        Assert.Single(result);
        Assert.Equal(TokenGroup.OPERATORS, result[0].TokenGroup);
        Assert.Equal(TokenType.PLUS, result[0].TokenType);
    }

    [Fact]
    public void ShouldMakeMinus()
    {
        //Arrange
        Fixture fixture = new();
        fixture.Customizations.Add(new TypeRelay(typeof(IFileWriteHandler), typeof(FileWriteHandler)));
        fixture.Customizations.Add(new TypeRelay(typeof(IEnvironmentVariables), typeof(EnvironmentVariables)));

        var environmentVariables = A.Fake<IEnvironmentVariables>();
        A.CallTo(() => environmentVariables.GetVariable("version")).Returns("1.0.0");
        var logger = A.Fake<Logger>();
        var tokenRepository = A.Fake<TokenRepository>();

        Lexer lexer = new(logger, tokenRepository);

        //Act
        var result = lexer.MakeTokens("-", " ");

        //Assert
        Assert.Single(result);
        Assert.Equal(TokenGroup.OPERATORS, result[0].TokenGroup);
        Assert.Equal(TokenType.MINUS, result[0].TokenType);
    }

    [Fact]
    public void ShouldMakeMultiply()
    {
        //Arrange
        Fixture fixture = new();
        fixture.Customizations.Add(new TypeRelay(typeof(IFileWriteHandler), typeof(FileWriteHandler)));
        fixture.Customizations.Add(new TypeRelay(typeof(IEnvironmentVariables), typeof(EnvironmentVariables)));

        var environmentVariables = A.Fake<IEnvironmentVariables>();
        A.CallTo(() => environmentVariables.GetVariable("version")).Returns("1.0.0");
        var logger = A.Fake<Logger>();
        var tokenRepository = A.Fake<TokenRepository>();

        Lexer lexer = new(logger, tokenRepository);

        //Act
        var result = lexer.MakeTokens("*", " ");

        //Assert
        Assert.Single(result);
        Assert.Equal(TokenGroup.OPERATORS, result[0].TokenGroup);
        Assert.Equal(TokenType.MULTIPLY, result[0].TokenType);
    }

    [Fact]
    public void ShouldMakeDivide()
    {
        //Arrange
        Fixture fixture = new();
        fixture.Customizations.Add(new TypeRelay(typeof(IFileWriteHandler), typeof(FileWriteHandler)));
        fixture.Customizations.Add(new TypeRelay(typeof(IEnvironmentVariables), typeof(EnvironmentVariables)));

        var environmentVariables = A.Fake<IEnvironmentVariables>();
        A.CallTo(() => environmentVariables.GetVariable("version")).Returns("1.0.0");
        var logger = A.Fake<Logger>();
        var tokenRepository = A.Fake<TokenRepository>();

        Lexer lexer = new(logger, tokenRepository);

        //Act
        var result = lexer.MakeTokens("/ ", " ");

        //Assert
        Assert.Single(result);
        Assert.Equal(TokenGroup.OPERATORS, result[0].TokenGroup);
        Assert.Equal(TokenType.DIVIDE, result[0].TokenType);
    }

    [Fact]
    public void ShouldMakePower()
    {
        //Arrange
        Fixture fixture = new();
        fixture.Customizations.Add(new TypeRelay(typeof(IFileWriteHandler), typeof(FileWriteHandler)));
        fixture.Customizations.Add(new TypeRelay(typeof(IEnvironmentVariables), typeof(EnvironmentVariables)));

        var environmentVariables = A.Fake<IEnvironmentVariables>();
        A.CallTo(() => environmentVariables.GetVariable("version")).Returns("1.0.0");
        var logger = A.Fake<Logger>();
        var tokenRepository = A.Fake<TokenRepository>();

        Lexer lexer = new(logger, tokenRepository);

        //Act
        var result = lexer.MakeTokens("^", " ");

        //Assert
        Assert.Single(result);
        Assert.Equal(TokenGroup.OPERATORS, result[0].TokenGroup);
        Assert.Equal(TokenType.POWER, result[0].TokenType);
    }

    [Fact]
    public void ShouldMakeLeftParentheses()
    {
        //Arrange
        Fixture fixture = new();
        fixture.Customizations.Add(new TypeRelay(typeof(IFileWriteHandler), typeof(FileWriteHandler)));
        fixture.Customizations.Add(new TypeRelay(typeof(IEnvironmentVariables), typeof(EnvironmentVariables)));

        var environmentVariables = A.Fake<IEnvironmentVariables>();
        A.CallTo(() => environmentVariables.GetVariable("version")).Returns("1.0.0");
        var logger = A.Fake<Logger>();
        var tokenRepository = A.Fake<TokenRepository>();

        Lexer lexer = new(logger, tokenRepository);

        //Act
        var result = lexer.MakeTokens("(", " ");

        //Assert
        Assert.Single(result);
        Assert.Equal(TokenGroup.SYNTAX, result[0].TokenGroup);
        Assert.Equal(TokenType.LEFTPARENTHESES, result[0].TokenType);
    }

    [Fact]
    public void ShouldMakeRightParentheses()
    {
        //Arrange
        Fixture fixture = new();
        fixture.Customizations.Add(new TypeRelay(typeof(IFileWriteHandler), typeof(FileWriteHandler)));
        fixture.Customizations.Add(new TypeRelay(typeof(IEnvironmentVariables), typeof(EnvironmentVariables)));

        var environmentVariables = A.Fake<IEnvironmentVariables>();
        A.CallTo(() => environmentVariables.GetVariable("version")).Returns("1.0.0");
        var logger = A.Fake<Logger>();
        var tokenRepository = A.Fake<TokenRepository>();

        Lexer lexer = new(logger, tokenRepository);

        //Act
        var result = lexer.MakeTokens(")", " ");

        //Assert
        Assert.Single(result);
        Assert.Equal(TokenGroup.SYNTAX, result[0].TokenGroup);
        Assert.Equal(TokenType.RIGHTPARENTHESES, result[0].TokenType);
    }

    [Fact]
    public void ShouldMakeLeftCurlyBrace()
    {
        //Arrange
        Fixture fixture = new();
        fixture.Customizations.Add(new TypeRelay(typeof(IFileWriteHandler), typeof(FileWriteHandler)));
        fixture.Customizations.Add(new TypeRelay(typeof(IEnvironmentVariables), typeof(EnvironmentVariables)));

        var environmentVariables = A.Fake<IEnvironmentVariables>();
        A.CallTo(() => environmentVariables.GetVariable("version")).Returns("1.0.0");
        var logger = A.Fake<Logger>();
        var tokenRepository = A.Fake<TokenRepository>();

        Lexer lexer = new(logger, tokenRepository);

        //Act
        var result = lexer.MakeTokens("{", " ");

        //Assert
        Assert.Single(result);
        Assert.Equal(TokenGroup.SYNTAX, result[0].TokenGroup);
        Assert.Equal(TokenType.LEFTCURLYBRACE, result[0].TokenType);
    }

    [Fact]
    public void ShouldMakeRightCurlyBrace()
    {
        //Arrange
        Fixture fixture = new();
        fixture.Customizations.Add(new TypeRelay(typeof(IFileWriteHandler), typeof(FileWriteHandler)));
        fixture.Customizations.Add(new TypeRelay(typeof(IEnvironmentVariables), typeof(EnvironmentVariables)));

        var environmentVariables = A.Fake<IEnvironmentVariables>();
        A.CallTo(() => environmentVariables.GetVariable("version")).Returns("1.0.0");
        var logger = A.Fake<Logger>();
        var tokenRepository = A.Fake<TokenRepository>();

        Lexer lexer = new(logger, tokenRepository);

        //Act
        var result = lexer.MakeTokens("}", " ");

        //Assert
        Assert.Single(result);
        Assert.Equal(TokenGroup.SYNTAX, result[0].TokenGroup);
        Assert.Equal(TokenType.RIGHTCURLYBRACE, result[0].TokenType);
    }

    [Fact]
    public void ShouldMakeComma()
    {
        //Arrange
        Fixture fixture = new();
        fixture.Customizations.Add(new TypeRelay(typeof(IFileWriteHandler), typeof(FileWriteHandler)));
        fixture.Customizations.Add(new TypeRelay(typeof(IEnvironmentVariables), typeof(EnvironmentVariables)));

        var environmentVariables = A.Fake<IEnvironmentVariables>();
        A.CallTo(() => environmentVariables.GetVariable("version")).Returns("1.0.0");
        var logger = A.Fake<Logger>();
        var tokenRepository = A.Fake<TokenRepository>();

        Lexer lexer = new(logger, tokenRepository);

        //Act
        var result = lexer.MakeTokens(",", " ");

        //Assert
        Assert.Single(result);
        Assert.Equal(TokenGroup.SYNTAX, result[0].TokenGroup);
        Assert.Equal(TokenType.COMMA, result[0].TokenType);
    }

    [Fact]
    public void ShouldMakeColon()
    {
        //Arrange
        Fixture fixture = new();
        fixture.Customizations.Add(new TypeRelay(typeof(IFileWriteHandler), typeof(FileWriteHandler)));
        fixture.Customizations.Add(new TypeRelay(typeof(IEnvironmentVariables), typeof(EnvironmentVariables)));

        var environmentVariables = A.Fake<IEnvironmentVariables>();
        A.CallTo(() => environmentVariables.GetVariable("version")).Returns("1.0.0");
        var logger = A.Fake<Logger>();
        var tokenRepository = A.Fake<TokenRepository>();

        Lexer lexer = new(logger, tokenRepository);

        //Act
        var result = lexer.MakeTokens(":", " ");

        //Assert
        Assert.Single(result);
        Assert.Equal(TokenGroup.SYNTAX, result[0].TokenGroup);
        Assert.Equal(TokenType.COLON, result[0].TokenType);
    }

    [Fact]
    public void ShouldMakeSemicolon()
    {
        //Arrange
        Fixture fixture = new();
        fixture.Customizations.Add(new TypeRelay(typeof(IFileWriteHandler), typeof(FileWriteHandler)));
        fixture.Customizations.Add(new TypeRelay(typeof(IEnvironmentVariables), typeof(EnvironmentVariables)));

        var environmentVariables = A.Fake<IEnvironmentVariables>();
        A.CallTo(() => environmentVariables.GetVariable("version")).Returns("1.0.0");
        var logger = A.Fake<Logger>();
        var tokenRepository = A.Fake<TokenRepository>();

        Lexer lexer = new(logger, tokenRepository);

        //Act
        var result = lexer.MakeTokens(";", " ");

        //Assert
        Assert.Single(result);
        Assert.Equal(TokenGroup.SYNTAX, result[0].TokenGroup);
        Assert.Equal(TokenType.SEMICOLON, result[0].TokenType);
    }

    [Fact]
    public void ShouldMakeDot()
    {
        //Arrange
        Fixture fixture = new();
        fixture.Customizations.Add(new TypeRelay(typeof(IFileWriteHandler), typeof(FileWriteHandler)));
        fixture.Customizations.Add(new TypeRelay(typeof(IEnvironmentVariables), typeof(EnvironmentVariables)));

        var environmentVariables = A.Fake<IEnvironmentVariables>();
        A.CallTo(() => environmentVariables.GetVariable("version")).Returns("1.0.0");
        var logger = A.Fake<Logger>();
        var tokenRepository = A.Fake<TokenRepository>();

        Lexer lexer = new(logger, tokenRepository);

        //Act
        var result = lexer.MakeTokens(".", " ");

        //Assert
        Assert.Single(result);
        Assert.Equal(TokenGroup.SYNTAX, result[0].TokenGroup);
        Assert.Equal(TokenType.DOT, result[0].TokenType);
    }

    [Fact]
    public void ShouldMakeLeftBracket()
    {
        //Arrange
        Fixture fixture = new();
        fixture.Customizations.Add(new TypeRelay(typeof(IFileWriteHandler), typeof(FileWriteHandler)));
        fixture.Customizations.Add(new TypeRelay(typeof(IEnvironmentVariables), typeof(EnvironmentVariables)));

        var environmentVariables = A.Fake<IEnvironmentVariables>();
        A.CallTo(() => environmentVariables.GetVariable("version")).Returns("1.0.0");
        var logger = A.Fake<Logger>();
        var tokenRepository = A.Fake<TokenRepository>();

        Lexer lexer = new(logger, tokenRepository);

        //Act
        var result = lexer.MakeTokens("[", " ");

        //Assert
        Assert.Single(result);
        Assert.Equal(TokenGroup.SYNTAX, result[0].TokenGroup);
        Assert.Equal(TokenType.LEFTBRACKET, result[0].TokenType);
    }

    [Fact]
    public void ShouldMakeRightBracket()
    {
        //Arrange
        Fixture fixture = new();
        fixture.Customizations.Add(new TypeRelay(typeof(IFileWriteHandler), typeof(FileWriteHandler)));
        fixture.Customizations.Add(new TypeRelay(typeof(IEnvironmentVariables), typeof(EnvironmentVariables)));

        var environmentVariables = A.Fake<IEnvironmentVariables>();
        A.CallTo(() => environmentVariables.GetVariable("version")).Returns("1.0.0");
        var logger = A.Fake<Logger>();
        var tokenRepository = A.Fake<TokenRepository>();

        Lexer lexer = new(logger, tokenRepository);

        //Act
        var result = lexer.MakeTokens("]", " ");

        //Assert
        Assert.Single(result);
        Assert.Equal(TokenGroup.SYNTAX, result[0].TokenGroup);
        Assert.Equal(TokenType.RIGHTBRACKET, result[0].TokenType);
    }

    [Fact]
    public void ShouldMakeEquals()
    {
        //Arrange
        Fixture fixture = new();
        fixture.Customizations.Add(new TypeRelay(typeof(IFileWriteHandler), typeof(FileWriteHandler)));
        fixture.Customizations.Add(new TypeRelay(typeof(IEnvironmentVariables), typeof(EnvironmentVariables)));

        var environmentVariables = A.Fake<IEnvironmentVariables>();
        A.CallTo(() => environmentVariables.GetVariable("version")).Returns("1.0.0");
        var logger = A.Fake<Logger>();
        var tokenRepository = A.Fake<TokenRepository>();

        Lexer lexer = new(logger, tokenRepository);

        //Act
        var result = lexer.MakeTokens("= ", " ");

        //Assert
        Assert.Single(result);
        Assert.Equal(TokenGroup.SYNTAX, result[0].TokenGroup);
        Assert.Equal(TokenType.EQUALS, result[0].TokenType);
    }

    [Fact]
    public void ShouldMakeLessThan()
    {
        //Arrange
        Fixture fixture = new();
        fixture.Customizations.Add(new TypeRelay(typeof(IFileWriteHandler), typeof(FileWriteHandler)));
        fixture.Customizations.Add(new TypeRelay(typeof(IEnvironmentVariables), typeof(EnvironmentVariables)));

        var environmentVariables = A.Fake<IEnvironmentVariables>();
        A.CallTo(() => environmentVariables.GetVariable("version")).Returns("1.0.0");
        var logger = A.Fake<Logger>();
        var tokenRepository = A.Fake<TokenRepository>();

        Lexer lexer = new(logger, tokenRepository);

        //Act
        var result = lexer.MakeTokens("< ", " ");

        //Assert
        Assert.Single(result);
        Assert.Equal(TokenGroup.COMPARISONOPERATORS, result[0].TokenGroup);
        Assert.Equal(TokenType.LESSTHAN, result[0].TokenType);
    }

    [Fact]
    public void ShouldMakeGreaterThan()
    {
        //Arrange
        Fixture fixture = new();
        fixture.Customizations.Add(new TypeRelay(typeof(IFileWriteHandler), typeof(FileWriteHandler)));
        fixture.Customizations.Add(new TypeRelay(typeof(IEnvironmentVariables), typeof(EnvironmentVariables)));

        var environmentVariables = A.Fake<IEnvironmentVariables>();
        A.CallTo(() => environmentVariables.GetVariable("version")).Returns("1.0.0");
        var logger = A.Fake<Logger>();
        var tokenRepository = A.Fake<TokenRepository>();

        Lexer lexer = new(logger, tokenRepository);

        //Act
        var result = lexer.MakeTokens("> ", " ");

        //Assert
        Assert.Single(result);
        Assert.Equal(TokenGroup.COMPARISONOPERATORS, result[0].TokenGroup);
        Assert.Equal(TokenType.GREATERTHAN, result[0].TokenType);
    }

    [Fact]
    public void ShouldMakeNotEquals()
    {
        //Arrange
        Fixture fixture = new();
        fixture.Customizations.Add(new TypeRelay(typeof(IFileWriteHandler), typeof(FileWriteHandler)));
        fixture.Customizations.Add(new TypeRelay(typeof(IEnvironmentVariables), typeof(EnvironmentVariables)));

        var environmentVariables = A.Fake<IEnvironmentVariables>();
        A.CallTo(() => environmentVariables.GetVariable("version")).Returns("1.0.0");
        var logger = A.Fake<Logger>();
        var tokenRepository = A.Fake<TokenRepository>();

        Lexer lexer = new(logger, tokenRepository);

        //Act
        var result = lexer.MakeTokens("!=", " ");

        //Assert
        Assert.Single(result);
        Assert.Equal(TokenGroup.COMPARISONOPERATORS, result[0].TokenGroup);
        Assert.Equal(TokenType.NOTEQUALS, result[0].TokenType);
    }


}
