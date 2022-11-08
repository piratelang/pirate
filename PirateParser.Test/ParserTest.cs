using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PirateParser.Node;
using PirateParser.Parsers;
using Xunit;

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
        
        var parser = new OperationParser(tokens, tokens[0], logger);

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
        
        var parser = new OperationParser(tokens, tokens[0], logger);

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
        
        var parser = new OperationParser(tokens, tokens[0], logger);

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

        var parser = new VariableAssignParser(tokens, tokens[0], logger, parserFactory);

        // Act
        var result = parser.CreateNode();

        // Assert
        Assert.IsType<VariableAssignNode>(result.node);
    }
}