
namespace PirateLexer.Test;

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
        Assert.Equal(1, result.Count);
        Assert.Equal(TokenGroup.VALUE, result[0].TokenGroup);
        Assert.Equal(TokenValue.INT, result[0].TokenType);
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
        Assert.Equal(1, result.Count);
        Assert.Equal(TokenGroup.VALUE, result[0].TokenGroup);
        Assert.Equal(TokenValue.FLOAT, result[0].TokenType);
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
        Assert.Equal(1, result.Count);
        Assert.Equal(TokenGroup.SYNTAX, result[0].TokenGroup);
        Assert.Equal(TokenSyntax.IDENTIFIER, result[0].TokenType);
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
        Assert.Equal(1, result.Count);
        Assert.Equal(TokenGroup.VALUE, result[0].TokenGroup);
        Assert.Equal(TokenValue.STRING, result[0].TokenType);
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
        Assert.Equal(1, result.Count);
        Assert.Equal(TokenGroup.VALUE, result[0].TokenGroup);
        Assert.Equal(TokenValue.CHAR, result[0].TokenType);
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
        Assert.Equal(1, result.Count);
        Assert.Equal(TokenGroup.OPERATORS, result[0].TokenGroup);
        Assert.Equal(TokenOperators.PLUS, result[0].TokenType);
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
        Assert.Equal(1, result.Count);
        Assert.Equal(TokenGroup.OPERATORS, result[0].TokenGroup);
        Assert.Equal(TokenOperators.MINUS, result[0].TokenType);
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
        Assert.Equal(1, result.Count);
        Assert.Equal(TokenGroup.OPERATORS, result[0].TokenGroup);
        Assert.Equal(TokenOperators.MULTIPLY, result[0].TokenType);
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
        Assert.Equal(1, result.Count);
        Assert.Equal(TokenGroup.OPERATORS, result[0].TokenGroup);
        Assert.Equal(TokenOperators.DIVIDE, result[0].TokenType);
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
        Assert.Equal(1, result.Count);
        Assert.Equal(TokenGroup.OPERATORS, result[0].TokenGroup);
        Assert.Equal(TokenOperators.POWER, result[0].TokenType);
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
        Assert.Equal(1, result.Count);
        Assert.Equal(TokenGroup.SYNTAX, result[0].TokenGroup);
        Assert.Equal(TokenSyntax.LEFTPARENTHESES, result[0].TokenType);
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
        Assert.Equal(1, result.Count);
        Assert.Equal(TokenGroup.SYNTAX, result[0].TokenGroup);
        Assert.Equal(TokenSyntax.RIGHTPARENTHESES, result[0].TokenType);
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
        Assert.Equal(1, result.Count);
        Assert.Equal(TokenGroup.SYNTAX, result[0].TokenGroup);
        Assert.Equal(TokenSyntax.LEFTCURLYBRACE, result[0].TokenType);
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
        Assert.Equal(1, result.Count);
        Assert.Equal(TokenGroup.SYNTAX, result[0].TokenGroup);
        Assert.Equal(TokenSyntax.RIGHTCURLYBRACE, result[0].TokenType);
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
        Assert.Equal(1, result.Count);
        Assert.Equal(TokenGroup.SYNTAX, result[0].TokenGroup);
        Assert.Equal(TokenSyntax.COMMA, result[0].TokenType);
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
        Assert.Equal(1, result.Count);
        Assert.Equal(TokenGroup.SYNTAX, result[0].TokenGroup);
        Assert.Equal(TokenSyntax.COLON, result[0].TokenType);
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
        Assert.Equal(1, result.Count);
        Assert.Equal(TokenGroup.SYNTAX, result[0].TokenGroup);
        Assert.Equal(TokenSyntax.SEMICOLON, result[0].TokenType);
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
        Assert.Equal(1, result.Count);
        Assert.Equal(TokenGroup.SYNTAX, result[0].TokenGroup);
        Assert.Equal(TokenSyntax.DOT, result[0].TokenType);
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
        Assert.Equal(1, result.Count);
        Assert.Equal(TokenGroup.SYNTAX, result[0].TokenGroup);
        Assert.Equal(TokenSyntax.LEFTBRACKET, result[0].TokenType);
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
        Assert.Equal(1, result.Count);
        Assert.Equal(TokenGroup.SYNTAX, result[0].TokenGroup);
        Assert.Equal(TokenSyntax.RIGHTBRACKET, result[0].TokenType);
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
        Assert.Equal(1, result.Count);
        Assert.Equal(TokenGroup.SYNTAX, result[0].TokenGroup);
        Assert.Equal(TokenSyntax.EQUALS, result[0].TokenType);
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
        Assert.Equal(1, result.Count);
        Assert.Equal(TokenGroup.COMPARISONOPERATORS, result[0].TokenGroup);
        Assert.Equal(TokenComparisonOperators.LESSTHAN, result[0].TokenType);
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
        Assert.Equal(1, result.Count);
        Assert.Equal(TokenGroup.COMPARISONOPERATORS, result[0].TokenGroup);
        Assert.Equal(TokenComparisonOperators.GREATERTHAN, result[0].TokenType);
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
        Assert.Equal(1, result.Count);
        Assert.Equal(TokenGroup.COMPARISONOPERATORS, result[0].TokenGroup);
        Assert.Equal(TokenComparisonOperators.NOTEQUALS, result[0].TokenType);
    }


}
