using System;
using System.Collections.Generic;
using Pirate.Interpreter.Interpreters;
using Pirate.Interpreter.Runtime;
using Pirate.Interpreter.StandarLibrary;
using Pirate.Interpreter.Values;
using Pirate.Interpreter.Values.Function;
using Pirate.Parser.Node;
using Pirate.Parser.Node.Interfaces;

namespace Pirate.Interpreter.Test;

public class InterpretersTest
{
    [Fact]
    public void ShouldInterpretBinaryOperationNode()
    {
        var (runtime, interpreterFactory, logger) = CreateContext();
        var node = new BinaryOperationNode(
            IntNode(1L),
            new Token(TokenGroup.OPERATORS, TokenType.PLUS),
            IntNode(1L));

        var result = new BinaryOperationInterpreter(node, interpreterFactory, logger).VisitNode();

        Assert.Single(result);
        Assert.IsType<IntegerValue>(result[0]);
        Assert.Equal(2L, result[0].Value);
    }

    [Fact]
    public void ShouldInterpretComparisonOperationNodeWithLessThanEquals()
    {
        var (_, interpreterFactory, logger) = CreateContext();
        var node = new ComparisonOperationNode(
            IntNode(1L),
            new Token(TokenGroup.COMPARISONOPERATORS, TokenType.LESSTHANEQUALS),
            IntNode(1L));

        var result = new ComparisonOperationInterpreter(node, interpreterFactory, logger).VisitNode();

        Assert.Single(result);
        Assert.IsType<BooleanValue>(result[0]);
        Assert.Equal(1L, result[0].Value);
    }

    [Fact]
    public void ShouldInterpretIfStatementNodeElseBranch()
    {
        var (_, interpreterFactory, logger) = CreateContext();
        var node = new IfStatementNode(
            new ComparisonOperationNode(IntNode(1L), new Token(TokenGroup.COMPARISONOPERATORS, TokenType.DOUBLEEQUALS), IntNode(2L)),
            new List<INode> { IntNode(10L) },
            new List<INode> { IntNode(20L) });

        var result = new IfStatementInterpreter(node, interpreterFactory, logger).VisitNode();

        Assert.Single(result);
        Assert.IsType<IntegerValue>(result[0]);
        Assert.Equal(20L, result[0].Value);
    }

    [Fact]
    public void ShouldInterpretWhileLoopStatementNode()
    {
        var (runtime, interpreterFactory, logger) = CreateContext();
        runtime.Variables.Set("i", new IntegerValue(0L, logger));
        var node = new WhileLoopStatementNode(
            new ComparisonOperationNode(IdentifierNode("i"), new Token(TokenGroup.COMPARISONOPERATORS, TokenType.LESSTHAN), IntNode(2L)),
            new List<INode>
            {
                new VariableAssignmentNode(
                    IdentifierNode("i"),
                    new BinaryOperationNode(IdentifierNode("i"), new Token(TokenGroup.OPERATORS, TokenType.PLUS), IntNode(1L)))
            });

        var result = new WhileLoopStatementInterpreter(node, interpreterFactory, logger).VisitNode();

        Assert.Equal(2, result.Count);
        Assert.Equal(2L, runtime.Variables.Get("i").Value);
    }

    [Fact]
    public void ShouldInterpretForLoopStatementNodeIncludingUpperBound()
    {
        var (_, interpreterFactory, logger) = CreateContext();
        var node = new ForLoopStatementNode(
            new VariableDeclarationNode(
                new Token(TokenGroup.TYPEKEYWORD, TokenType.VAR),
                IdentifierNode("i"),
                IntNode(0L)),
            IntNode(2L),
            new List<INode> { IntNode(1L) });

        var result = new ForLoopStatementInterpreter(node, interpreterFactory, logger, CreateRuntime(logger)).VisitNode();

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void ShouldInterpretFunctionDeclarationNode()
    {
        var (runtime, interpreterFactory, logger) = CreateContext();
        var node = new FunctionDeclarationNode(
            IdentifierNode("answer"),
            new(),
            new Token(TokenGroup.TYPEKEYWORD, TokenType.VOID),
            new(),
            IntNode(42L));

        var result = new FunctionDeclarationInterpreter(node, interpreterFactory, logger, runtime).VisitNode();

        Assert.Empty(result);
        Assert.IsType<FunctionValue>(runtime.Functions.Get("answer"));
    }

    [Fact]
    public void ShouldInterpretFunctionCallNode()
    {
        var (runtime, interpreterFactory, logger) = CreateContext();
        var functionNode = new FunctionDeclarationNode(
            IdentifierNode("answer"),
            new(),
            new Token(TokenGroup.TYPEKEYWORD, TokenType.INT),
            new(),
            IntNode(42L));
        runtime.Functions.Set("answer", new FunctionValue(functionNode, logger));
        var callNode = new FunctionCallNode(IdentifierNode("answer"), new());

        var result = new FunctionCallInterpreter(callNode, interpreterFactory, logger, new StandardLibraryProvider(logger), runtime).VisitNode();

        Assert.Single(result);
        Assert.Equal(42L, result[0].Value);
    }

    [Fact]
    public void ShouldInterpretVariableDeclarationNode()
    {
        var (runtime, interpreterFactory, logger) = CreateContext();
        var node = new VariableDeclarationNode(
            new Token(TokenGroup.TYPEKEYWORD, TokenType.VAR),
            IdentifierNode("a"),
            IntNode(1L));

        var result = new VariableDeclarationInterpreter(node, interpreterFactory, logger, runtime).VisitNode();

        Assert.Single(result);
        Assert.IsType<VariableValue>(result[0]);
        Assert.Equal(1L, runtime.Variables.Get("a").Value);
    }

    [Fact]
    public void ShouldInterpretVariableAssignmentNode()
    {
        var (runtime, interpreterFactory, logger) = CreateContext();
        runtime.Variables.Set("a", new IntegerValue(1L, logger));
        var node = new VariableAssignmentNode(IdentifierNode("a"), IntNode(2L));

        var result = new VariableAssignmentInterpreter(node, logger, interpreterFactory, runtime).VisitNode();

        Assert.Single(result);
        Assert.IsType<VariableValue>(result[0]);
        Assert.Equal(2L, runtime.Variables.Get("a").Value);
    }

    [Fact]
    public void ShouldInterpretValueNodeIdentifier()
    {
        var (runtime, interpreterFactory, logger) = CreateContext();
        runtime.Variables.Set("a", new IntegerValue(5L, logger));
        var node = IdentifierNode("a");

        var result = new ValueInterpreter(node, interpreterFactory, logger, runtime).VisitNode();

        Assert.Single(result);
        Assert.IsType<VariableValue>(result[0]);
        Assert.Equal(5L, result[0].Value);
    }

    [Fact]
    public void ShouldInterpretExternNode()
    {
        var (runtime, interpreterFactory, logger) = CreateContext();
        var node = new ExternNode(IdentifierNode("print"));

        var result = new ExternInterpreter(node, runtime, new StandardLibraryProvider(logger), logger, interpreterFactory).VisitNode();

        Assert.Empty(result);
        Assert.IsAssignableFrom<CSharpFunction>(runtime.Functions.Get("print"));
    }

    [Fact]
    public void ShouldInterpretCommentNode()
    {
        var (_, interpreterFactory, logger) = CreateContext();
        var node = new CommentNode(new List<Token> { new(TokenGroup.SYNTAX, TokenType.IDENTIFIER, "hello") });

        var result = new CommentInterpreter(node, interpreterFactory, logger).VisitNode();

        Assert.Empty(result);
    }

    [Fact]
    public void ShouldCreateInterpreterForEveryInterpreterNode()
    {
        var (runtime, interpreterFactory, logger) = CreateContext();
        var nodes = new Dictionary<INode, Type>
        {
            { new BinaryOperationNode(IntNode(1L), new Token(TokenGroup.OPERATORS, TokenType.PLUS), IntNode(1L)), typeof(BinaryOperationInterpreter) },
            { new ComparisonOperationNode(IntNode(1L), new Token(TokenGroup.COMPARISONOPERATORS, TokenType.DOUBLEEQUALS), IntNode(1L)), typeof(ComparisonOperationInterpreter) },
            { new IfStatementNode(new ComparisonOperationNode(IntNode(1L), new Token(TokenGroup.COMPARISONOPERATORS, TokenType.DOUBLEEQUALS), IntNode(1L)), new List<INode> { IntNode(1L) }), typeof(IfStatementInterpreter) },
            { new WhileLoopStatementNode(new ComparisonOperationNode(IntNode(1L), new Token(TokenGroup.COMPARISONOPERATORS, TokenType.DOUBLEEQUALS), IntNode(1L)), new List<INode> { IntNode(1L) }), typeof(WhileLoopStatementInterpreter) },
            { new ForLoopStatementNode(new VariableDeclarationNode(new Token(TokenGroup.TYPEKEYWORD, TokenType.VAR), IdentifierNode("i"), IntNode(0L)), IntNode(1L), new List<INode> { IntNode(1L) }), typeof(ForLoopStatementInterpreter) },
            { new FunctionCallNode(IdentifierNode("answer"), new List<INode>()), typeof(FunctionCallInterpreter) },
            { new FunctionDeclarationNode(IdentifierNode("answer"), new List<IParameterDefinitionNode>(), new Token(TokenGroup.TYPEKEYWORD, TokenType.INT), new List<INode>(), IntNode(42L)), typeof(FunctionDeclarationInterpreter) },
            { new VariableDeclarationNode(new Token(TokenGroup.TYPEKEYWORD, TokenType.VAR), IdentifierNode("a"), IntNode(1L)), typeof(VariableDeclarationInterpreter) },
            { new VariableAssignmentNode(IdentifierNode("a"), IntNode(1L)), typeof(VariableAssignmentInterpreter) },
            { IntNode(1L), typeof(ValueInterpreter) },
            { new ExternNode(IdentifierNode("print")), typeof(ExternInterpreter) },
            { new CommentNode(new List<Token>()), typeof(CommentInterpreter) }
        };

        foreach (var (node, expectedType) in nodes)
        {
            Assert.IsType(expectedType, interpreterFactory.GetInterpreter(node));
        }
    }

    private static (Runtime.Runtime Runtime, InterpreterFactory InterpreterFactory, ILogger Logger) CreateContext()
    {
        var logger = A.Fake<ILogger>();
        var runtime = CreateRuntime(logger);
        var interpreterFactory = new InterpreterFactory(new StandardLibraryProvider(logger), logger, runtime);
        return (runtime, interpreterFactory, logger);
    }

    private static Runtime.Runtime CreateRuntime(ILogger logger) => new(logger);

    private static ValueNode IntNode(long value) => new(new Token(TokenGroup.VALUE, TokenType.INT, value));

    private static ValueNode IdentifierNode(string name) => new(new Token(TokenGroup.SYNTAX, TokenType.IDENTIFIER, name));
}
