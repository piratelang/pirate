using System.Collections.Generic;

namespace Pirate.Parser.Test;

public class ParserTest
{
    [Fact]
    public void ShouldReturnBinaryOperationNode()
    {
        // Arrange
        var logger = A.Fake<ILogger>();

        var tokens = new List<Token>();
        tokens.Add(new Token(TokenGroup.VALUE, TokenType.INT, "1"));
        tokens.Add(new Token(TokenGroup.OPERATORS, TokenType.PLUS, "+"));
        tokens.Add(new Token(TokenGroup.VALUE, TokenType.INT, "1"));

        var parserFactory = new ParserFactory();
        var parser = parserFactory.GetParser(0, tokens, logger);

        // Act
        var result = parser.CreateNode();

        // Assert
        Assert.IsType<BinaryOperationNode>(result.Node);
    }

    [Fact]
    public void ShouldReturnComparisonOperationNode()
    {
        // Arrange
        var logger = A.Fake<ILogger>();

        var tokens = new List<Token>();
        tokens.Add(new Token(TokenGroup.VALUE, TokenType.INT, "1"));
        tokens.Add(new Token(TokenGroup.COMPARISONOPERATORS, TokenType.DOUBLEEQUALS, "=="));
        tokens.Add(new Token(TokenGroup.VALUE, TokenType.INT, "1"));

        var parserFactory = new ParserFactory();
        var parser = parserFactory.GetParser(0, tokens, logger);

        // Act
        var result = parser.CreateNode();

        // Assert
        Assert.IsType<ComparisonOperationNode>(result.Node);
    }

    [Fact]
    public void ShouldReturnValueNode()
    {
        // Arrange
        var logger = A.Fake<ILogger>();

        var tokens = new List<Token>();
        tokens.Add(new Token(TokenGroup.VALUE, TokenType.INT, "1"));

        var parserFactory = new ParserFactory();
        var parser = parserFactory.GetParser(0, tokens, logger);

        // Act
        var result = parser.CreateNode();

        // Assert
        Assert.IsType<ValueNode>(result.Node);
    }

    [Fact]
    public void ShouldReturnVariableAssignNode()
    {
        // Arrange
        var logger = A.Fake<ILogger>();

        var tokens = new List<Token>();
        tokens.Add(new Token(TokenGroup.TYPEKEYWORD, TokenType.VAR));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.IDENTIFIER, "a"));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.EQUALS));
        tokens.Add(new Token(TokenGroup.VALUE, TokenType.INT, "1"));
        var parserFactory = new ParserFactory();
        var parser = parserFactory.GetParser(0, tokens, logger);

        // Act
        var result = parser.CreateNode();

        // Assert
        Assert.IsType<VariableDeclarationNode>(result.Node);
    }

    [Fact]
    public void ShouldReturnIfStatementNode()
    {
        // Arrange
        var logger = A.Fake<ILogger>();

        var tokens = new List<Token>();
        tokens.Add(new Token(TokenGroup.CONTROLKEYWORD, TokenType.IF));
        tokens.Add(new Token(TokenGroup.VALUE, TokenType.INT, "1"));
        tokens.Add(new Token(TokenGroup.COMPARISONOPERATORS, TokenType.DOUBLEEQUALS, "=="));
        tokens.Add(new Token(TokenGroup.VALUE, TokenType.INT, "1"));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.LEFTCURLYBRACE));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.RIGHTCURLYBRACE));

        var parserFactory = new ParserFactory();
        var parser = parserFactory.GetParser(0, tokens, logger);

        // Act
        var result = parser.CreateNode();

        // Assert
        Assert.IsType<IfStatementNode>(result.Node);
    }

    [Fact]
    public void ShouldReturnIfStatementNodeWithElseNodes()
    {
        // Arrange
        var logger = A.Fake<ILogger>();

        var tokens = new List<Token>();
        tokens.Add(new Token(TokenGroup.CONTROLKEYWORD, TokenType.IF));
        tokens.Add(new Token(TokenGroup.VALUE, TokenType.INT, "1"));
        tokens.Add(new Token(TokenGroup.COMPARISONOPERATORS, TokenType.DOUBLEEQUALS, "=="));
        tokens.Add(new Token(TokenGroup.VALUE, TokenType.INT, "1"));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.LEFTCURLYBRACE));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.RIGHTCURLYBRACE));
        tokens.Add(new Token(TokenGroup.CONTROLKEYWORD, TokenType.ELSE));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.LEFTCURLYBRACE));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.RIGHTCURLYBRACE));

        var parserFactory = new ParserFactory();
        var parser = parserFactory.GetParser(0, tokens, logger);

        // Act
        var result = parser.CreateNode();

        // Assert
        Assert.IsType<IfStatementNode>(result.Node);
        Assert.NotNull(((IfStatementNode)result.Node).ElseNode);
    }

    [Fact]
    public void ShouldReturnWhileLoopStatementNode()
    {
        // Arrange
        var logger = A.Fake<ILogger>();

        var tokens = new List<Token>();
        tokens.Add(new Token(TokenGroup.CONTROLKEYWORD, TokenType.WHILE));
        tokens.Add(new Token(TokenGroup.VALUE, TokenType.INT, "1"));
        tokens.Add(new Token(TokenGroup.COMPARISONOPERATORS, TokenType.DOUBLEEQUALS, "=="));
        tokens.Add(new Token(TokenGroup.VALUE, TokenType.INT, "1"));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.LEFTCURLYBRACE));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.RIGHTCURLYBRACE));

        var parserFactory = new ParserFactory();
        var parser = parserFactory.GetParser(0, tokens, logger);

        // Act
        var result = parser.CreateNode();

        // Assert
        Assert.IsType<WhileLoopStatementNode>(result.Node);
    }

    [Fact]
    public void ShouldReturnForLoopStatementNode()
    {
        // Arrange
        var logger = A.Fake<ILogger>();

        var tokens = new List<Token>();
        tokens.Add(new Token(TokenGroup.CONTROLKEYWORD, TokenType.FOR));
        tokens.Add(new Token(TokenGroup.TYPEKEYWORD, TokenType.VAR));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.IDENTIFIER, "a"));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.EQUALS));
        tokens.Add(new Token(TokenGroup.VALUE, TokenType.INT, "1"));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.TO));
        tokens.Add(new Token(TokenGroup.VALUE, TokenType.INT, "10"));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.LEFTCURLYBRACE));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.RIGHTCURLYBRACE));

        var parserFactory = new ParserFactory();
        var parser = parserFactory.GetParser(0, tokens, logger);

        // Act
        var result = parser.CreateNode();

        // Assert
        Assert.IsType<ForLoopStatementNode>(result.Node);
    }

    [Fact]
    public void ShouldReturnFunctionDeclarationNode()
    {
        // Arrange
        var logger = A.Fake<ILogger>();

        var tokens = new List<Token>();
        tokens.Add(new Token(TokenGroup.TYPEKEYWORD, TokenType.FUNC));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.IDENTIFIER, "a"));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.LEFTPARENTHESES));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.RIGHTPARENTHESES));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.COLON));
        tokens.Add(new Token(TokenGroup.TYPEKEYWORD, TokenType.VOID));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.LEFTCURLYBRACE));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.RIGHTCURLYBRACE));

        var parserFactory = new ParserFactory();
        var parser = parserFactory.GetParser(0, tokens, logger);

        // Act
        var result = parser.CreateNode();

        // Assert
        Assert.IsType<FunctionDeclarationNode>(result.Node);
    }

    [Fact]
    public void ShouldReturnFunctionDeclarationNodeWithReturnStatement()
    {
        // Arrange
        var logger = A.Fake<ILogger>();

        var tokens = new List<Token>();
        tokens.Add(new Token(TokenGroup.TYPEKEYWORD, TokenType.FUNC));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.IDENTIFIER, "a"));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.LEFTPARENTHESES));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.RIGHTPARENTHESES));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.COLON));
        tokens.Add(new Token(TokenGroup.TYPEKEYWORD, TokenType.VOID));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.LEFTCURLYBRACE));
        tokens.Add(new Token(TokenGroup.CONTROLKEYWORD, TokenType.RETURN));
        tokens.Add(new Token(TokenGroup.VALUE, TokenType.INT, "1"));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.SEMICOLON));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.RIGHTCURLYBRACE));

        var parserFactory = new ParserFactory();
        var parser = parserFactory.GetParser(0, tokens, logger);

        // Act
        var result = parser.CreateNode();

        // Assert
        Assert.IsType<FunctionDeclarationNode>(result.Node);
    }

    [Fact]
    public void ShouldReturnFunctionCallNode()
    {
        // Arrange
        var logger = A.Fake<ILogger>();

        var tokens = new List<Token>();
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.IDENTIFIER, "a"));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.LEFTPARENTHESES));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.RIGHTPARENTHESES));

        var parserFactory = new ParserFactory();
        var parser = parserFactory.GetParser(0, tokens, logger);

        // Act
        var result = parser.CreateNode();

        // Assert
        Assert.IsType<FunctionCallNode>(result.Node);
    }

    [Fact]
    public void ShouldReturnFunctionCallNodeWithParameters()
    {
        // Arrange
        var logger = A.Fake<ILogger>();

        var tokens = new List<Token>();
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.IDENTIFIER, "a"));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.LEFTPARENTHESES));
        tokens.Add(new Token(TokenGroup.VALUE, TokenType.INT, "1"));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.COMMA));
        tokens.Add(new Token(TokenGroup.VALUE, TokenType.INT, "2"));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.RIGHTPARENTHESES));

        var parserFactory = new ParserFactory();
        var parser = parserFactory.GetParser(0, tokens, logger);

        // Act
        var result = parser.CreateNode();

        // Assert
        Assert.IsType<FunctionCallNode>(result.Node);
    }

    [Fact]
    public void ShouldReturnFunctionCallNodeWithParametersAndReturnStatement()
    {
        // Arrange
        var logger = A.Fake<ILogger>();

        var tokens = new List<Token>();
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.IDENTIFIER, "a"));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.LEFTPARENTHESES));
        tokens.Add(new Token(TokenGroup.VALUE, TokenType.INT, "1"));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.COMMA));
        tokens.Add(new Token(TokenGroup.VALUE, TokenType.INT, "2"));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.RIGHTPARENTHESES));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.SEMICOLON));

        var parserFactory = new ParserFactory();
        var parser = parserFactory.GetParser(0, tokens, logger);

        // Act
        var result = parser.CreateNode();

        // Assert
        Assert.IsType<FunctionCallNode>(result.Node);
    }

    [Fact]
    public void ShouldReturnCommentNode()
    {
        // Arrange
        var logger = A.Fake<ILogger>();

        var tokens = new List<Token>();
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.DOUBLEDIVIDE));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.IDENTIFIER, "a"));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.SEMICOLON));

        var parserFactory = new ParserFactory();
        var parser = parserFactory.GetParser(0, tokens, logger);

        // Act
        var result = parser.CreateNode();

        // Assert
        Assert.IsType<CommentNode>(result.Node);
    }
}