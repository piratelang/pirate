using System.Collections.Generic;

namespace PirateParser.Test;

public class NodeTest
{
    [Fact]
    public void BinaryOperationNodeIsValid()
    {
        // Arrange
        var logger = A.Fake<ILogger>();
        var left = A.Fake<INode>();
        var right = A.Fake<INode>();
        var tokenoperator = A.Fake<Token>();
        tokenoperator.TokenType = TokenType.PLUS;
        var node = new BinaryOperationNode(left, tokenoperator, right);

        // Act
        var result = node.IsValid();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void BinaryOperationNodeIsNotValid()
    {
        // Arrange
        var logger = A.Fake<ILogger>();
        var left = A.Fake<INode>();
        var right = A.Fake<INode>();
        var tokenoperator = A.Fake<Token>();
        tokenoperator.TokenType = TokenType.PLUS;
        var node = new BinaryOperationNode(left, tokenoperator, right);
        node.Left = null;

        // Act
        var result = node.IsValid();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ComparisonOperationNodeIsValid()
    {
        // Arrange
        var logger = A.Fake<ILogger>();
        var left = A.Fake<INode>();
        var right = A.Fake<INode>();
        var tokenoperator = A.Fake<Token>();
        tokenoperator.TokenType = TokenType.DOUBLEEQUALS;
        var node = new ComparisonOperationNode(left, tokenoperator, right);

        // Act
        var result = node.IsValid();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ComparisonOperationNodeIsNotValid()
    {
        // Arrange
        var logger = A.Fake<ILogger>();
        var left = A.Fake<INode>();
        var right = A.Fake<INode>();
        var tokenoperator = A.Fake<Token>();
        tokenoperator.TokenType = TokenType.DOUBLEEQUALS;
        var node = new ComparisonOperationNode(left, tokenoperator, right);
        node.Left = null;

        // Act
        var result = node.IsValid();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValueNodeIsValid()
    {
        // Arrange
        var logger = A.Fake<ILogger>();
        var token = A.Fake<Token>();
        token.TokenType = TokenType.INT;
        token.Value = 1;
        var node = new ValueNode(token);

        // Act
        var result = node.IsValid();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValueNodeIsNotValid()
    {
        // Arrange
        var logger = A.Fake<ILogger>();
        var token = A.Fake<Token>();
        token.TokenType = TokenType.INT;
        token.Value = 1;
        var node = new ValueNode(token);
        node.Value = null;

        // Act
        var result = node.IsValid();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void VariableAssignNodeIsValid()
    {
        // Arrange
        var logger = A.Fake<ILogger>();
        var typetoken = A.Fake<Token>();
        typetoken.TokenType = TokenType.INT;
        var identifier = A.Fake<IValueNode>();
        var value = A.Fake<INode>();

        var node = new VariableAssignmentNode(identifier, value);

        // Act
        var result = node.IsValid();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void VariableAssignNodeIsNotValid()
    {
        // Arrange
        var logger = A.Fake<ILogger>();
        var typetoken = A.Fake<Token>();
        typetoken.TokenType = TokenType.INT;
        var identifier = A.Fake<IValueNode>();
        var value = A.Fake<INode>();

        var node = new VariableAssignmentNode(identifier, value);
        node.Value = null;

        // Act
        var result = node.IsValid();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IfStatementNodeIsValid()
    {
        // Arrange
        var logger = A.Fake<ILogger>();
        var condition = A.Fake<IOperationNode>();
        var body = A.Fake<List<INode>>();

        var node = new IfStatementNode(condition, body);

        // Act
        var result = node.IsValid();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IfStatementNodeIsNotValid()
    {
        // Arrange
        var logger = A.Fake<ILogger>();
        var condition = A.Fake<IOperationNode>();
        var body = A.Fake<List<INode>>();

        var node = new IfStatementNode(condition, body);
        node.ConditionNode = null;

        // Act
        var result = node.IsValid();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void WhileLoopStatementNodeIsValid()
    {
        // Arrange
        var logger = A.Fake<ILogger>();
        var condition = A.Fake<IOperationNode>();
        var body = A.Fake<List<INode>>();

        var node = new WhileLoopStatementNode(condition, body);

        // Act
        var result = node.IsValid();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void WhileLoopStatementNodeIsNotValid()
    {
        // Arrange
        var logger = A.Fake<ILogger>();
        var condition = A.Fake<IOperationNode>();
        var body = A.Fake<List<INode>>();

        var node = new WhileLoopStatementNode(condition, body);
        node.ConditionNode = null;

        // Act
        var result = node.IsValid();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ForLoopStatementNodeIsValid()
    {
        // Arrange
        var logger = A.Fake<ILogger>();
        var variableAssignNode = A.Fake<IVariableDeclarationNode>();
        var valueNode = A.Fake<IValueNode>();
        var body = A.Fake<List<INode>>();

        var node = new ForLoopStatementNode(variableAssignNode, valueNode, body);

        // Act
        var result = node.IsValid();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ForLoopStatementNodeIsNotValid()
    {
        // Arrange
        var logger = A.Fake<ILogger>();
        var variableAssignNode = A.Fake<IVariableDeclarationNode>();
        var valueNode = A.Fake<IValueNode>();
        var body = A.Fake<List<INode>>();

        var node = new ForLoopStatementNode(variableAssignNode, valueNode, body);
        node.VariableNode = null;

        // Act
        var result = node.IsValid();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void FunctionDeclarationNodeIsValid()
    {
        // Arrange
        var logger = A.Fake<ILogger>();
        var identifier = A.Fake<ValueNode>();
        var parameters = A.Fake<List<IParameterDefinitionNode>>();
        var returnTypeToken = A.Fake<Token>();
        var statements = A.Fake<List<INode>>();

        var node = new FunctionDeclarationNode(identifier, parameters, returnTypeToken, statements);

        // Act
        var result = node.IsValid();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void FunctionDeclarationNodeIsNotValid()
    {
        // Arrange
        var logger = A.Fake<ILogger>();
        var identifier = A.Fake<ValueNode>();
        var parameters = A.Fake<List<IParameterDefinitionNode>>();
        var returnTypeToken = A.Fake<Token>();
        var statements = A.Fake<List<INode>>();

        var node = new FunctionDeclarationNode(identifier, parameters, returnTypeToken, statements);
        node.Identifier = null;

        // Act
        var result = node.IsValid();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void FunctionCallNodeIsValid()
    {
        // Arrange
        var logger = A.Fake<ILogger>();
        var identifier = A.Fake<ValueNode>();
        var parameters = A.Fake<List<INode>>();

        var node = new FunctionCallNode(identifier, parameters);

        // Act
        var result = node.IsValid();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void FunctionCallNodeIsNotValid()
    {
        // Arrange
        var logger = A.Fake<ILogger>();
        var identifier = A.Fake<ValueNode>();
        var parameters = A.Fake<List<INode>>();

        var node = new FunctionCallNode(identifier, parameters);
        node.Identifier = null;

        // Act
        var result = node.IsValid();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ParameterDefinitionNodeIsValid()
    {
        // Arrange
        var logger = A.Fake<ILogger>();
        var identifier = A.Fake<ValueNode>();
        var typeToken = A.Fake<Token>();

        var node = new ParameterDefinitionNode(typeToken, identifier);

        // Act
        var result = node.IsValid();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ParameterDefinitionNodeIsNotValid()
    {
        // Arrange
        var logger = A.Fake<ILogger>();
        var identifier = A.Fake<ValueNode>();
        var typeToken = A.Fake<Token>();

        var node = new ParameterDefinitionNode(typeToken, identifier);
        node.Identifier = null;

        // Act
        var result = node.IsValid();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void VariableDeclarationNodeIsValid()
    {
        // Arrange
        var logger = A.Fake<ILogger>();
        var identifier = A.Fake<ValueNode>();
        var typeToken = A.Fake<Token>();
        typeToken.TokenType = TokenType.STRING;
        typeToken.TokenGroup = TokenGroup.TYPEKEYWORD;
        var valueNode = A.Fake<IValueNode>();

        var node = new VariableDeclarationNode(typeToken, identifier, valueNode);

        // Act
        var result = node.IsValid();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void VariableDeclarationNodeIsNotValid()
    {
        // Arrange
        var logger = A.Fake<ILogger>();
        var identifier = A.Fake<ValueNode>();
        var typeToken = A.Fake<Token>();
        var valueNode = A.Fake<IValueNode>();

        var node = new VariableDeclarationNode(typeToken, identifier, valueNode);
        node.Identifier = null;

        // Act
        var result = node.IsValid();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CommentNodeIsValid()
    {
        // Arrange
        var logger = A.Fake<ILogger>();
        var tokenList = A.Fake<List<Token>>();

        var node = new CommentNode(tokenList);

        // Act
        var result = node.IsValid();

        // Assert
        Assert.True(result);
    }
}