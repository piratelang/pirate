using System.Collections.Generic;

namespace PirateParser.Test;

public class ParserTest
{
    [Fact]
    public void ShouldReturnBinaryOperationNode()
    {
        // Arrange
        var logger = A.Fake<ILogger>();
        
        var tokens = new List<Token>();
        tokens.Add(new Token(TokenGroup.VALUE, TokenValue.INT, "1"));
        tokens.Add(new Token(TokenGroup.OPERATORS, TokenOperators.PLUS, "+"));
        tokens.Add(new Token(TokenGroup.VALUE, TokenValue.INT, "1"));
        
        var parserFactory = new ParserFactory();
        var parser = parserFactory.GetParser(tokens[0], tokens, logger);

        // Act
        var result = parser.CreateNode();

        // Assert
        Assert.IsType<BinaryOperationNode>(result.node);
    }

    [Fact]
    public void ShouldReturnComparisonOperationNode()
    {
        // Arrange
        var logger = A.Fake<ILogger>();
        
        var tokens = new List<Token>();
        tokens.Add(new Token(TokenGroup.VALUE, TokenValue.INT, "1"));
        tokens.Add(new Token(TokenGroup.COMPARISONOPERATORS, TokenComparisonOperators.DOUBLEEQUALS, "=="));
        tokens.Add(new Token(TokenGroup.VALUE, TokenValue.INT, "1"));
        
        var parserFactory = new ParserFactory();
        var parser = parserFactory.GetParser(tokens[0], tokens, logger);

        // Act
        var result = parser.CreateNode();

        // Assert
        Assert.IsType<ComparisonOperationNode>(result.node);
    }

    [Fact]
    public void ShouldReturnValueNode()
    {
        // Arrange
        var logger = A.Fake<ILogger>();
        
        var tokens = new List<Token>();
        tokens.Add(new Token(TokenGroup.VALUE, TokenValue.INT, "1"));
        
        var parserFactory = new ParserFactory();
        var parser = parserFactory.GetParser(tokens[0], tokens, logger);

        // Act
        var result = parser.CreateNode();

        // Assert
        Assert.IsType<ValueNode>(result.node);
    }

    [Fact]
    public void ShouldReturnVariableAssignNode()
    {
        // Arrange
        var logger = A.Fake<ILogger>();
        
        var tokens = new List<Token>();
        tokens.Add(new Token(TokenGroup.TYPEKEYWORD, TokenTypeKeyword.VAR));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.IDENTIFIER, "a"));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.EQUALS));
        tokens.Add(new Token(TokenGroup.VALUE, TokenValue.INT, "1"));
        var parserFactory = new ParserFactory();
        var parser = parserFactory.GetParser(tokens[0], tokens, logger);

        // Act
        var result = parser.CreateNode();

        // Assert
        Assert.IsType<VariableAssignNode>(result.node);
    }

    [Fact]
    public void ShouldReturnIfStatementNode()
    {
        // Arrange
        var logger = A.Fake<ILogger>();
        
        var tokens = new List<Token>();
        tokens.Add(new Token(TokenGroup.CONTROLKEYWORD, TokenControlKeyword.IF));
        tokens.Add(new Token(TokenGroup.VALUE, TokenValue.INT, "1"));
        tokens.Add(new Token(TokenGroup.COMPARISONOPERATORS, TokenComparisonOperators.DOUBLEEQUALS, "=="));
        tokens.Add(new Token(TokenGroup.VALUE, TokenValue.INT, "1"));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.LEFTCURLYBRACE));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.RIGHTCURLYBRACE));
        
        var parserFactory = new ParserFactory();
        var parser = parserFactory.GetParser(tokens[0], tokens, logger);

        // Act
        var result = parser.CreateNode();

        // Assert
        Assert.IsType<IfStatementNode>(result.node);
    }

    [Fact]
    public void ShouldReturnIfStatementNodeWithElseNodes()
    {
                // Arrange
        var logger = A.Fake<ILogger>();
        
        var tokens = new List<Token>();
        tokens.Add(new Token(TokenGroup.CONTROLKEYWORD, TokenControlKeyword.IF));
        tokens.Add(new Token(TokenGroup.VALUE, TokenValue.INT, "1"));
        tokens.Add(new Token(TokenGroup.COMPARISONOPERATORS, TokenComparisonOperators.DOUBLEEQUALS, "=="));
        tokens.Add(new Token(TokenGroup.VALUE, TokenValue.INT, "1"));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.LEFTCURLYBRACE));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.RIGHTCURLYBRACE));
        tokens.Add(new Token(TokenGroup.CONTROLKEYWORD, TokenControlKeyword.ELSE));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.LEFTCURLYBRACE));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.RIGHTCURLYBRACE));
        
        var parserFactory = new ParserFactory();
        var parser = parserFactory.GetParser(tokens[0], tokens, logger);

        // Act
        var result = parser.CreateNode();

        // Assert
        Assert.IsType<IfStatementNode>(result.node);
        Assert.NotNull(((IfStatementNode)result.node).ElseNode);
    }

    [Fact]
    public void ShouldReturnWhileLoopStatementNode()
    {
        // Arrange
        var logger = A.Fake<ILogger>();
        
        var tokens = new List<Token>();
        tokens.Add(new Token(TokenGroup.CONTROLKEYWORD, TokenControlKeyword.WHILE));
        tokens.Add(new Token(TokenGroup.VALUE, TokenValue.INT, "1"));
        tokens.Add(new Token(TokenGroup.COMPARISONOPERATORS, TokenComparisonOperators.DOUBLEEQUALS, "=="));
        tokens.Add(new Token(TokenGroup.VALUE, TokenValue.INT, "1"));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.LEFTCURLYBRACE));
        tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.RIGHTCURLYBRACE));
        
        var parserFactory = new ParserFactory();
        var parser = parserFactory.GetParser(tokens[0], tokens, logger);

        // Act
        var result = parser.CreateNode();

        // Assert
        Assert.IsType<WhileLoopStatementNode>(result.node);
    }
}