using System;
using System.Collections.Generic;
using PirateInterpreter.Interpreters;
using PirateInterpreter.StandardLibrary;
using PirateInterpreter.StandardLibrary.Interfaces;
using PirateInterpreter.Values;
using PirateParser.Node;
using PirateParser.Node.Interfaces;

namespace PirateInterpreter.Test;

public class InterpretersTest
{
    [Fact]
    public void ShouldInterpretBinaryOperationNode()
    {
        // Arrange
        var binaryOperationNode = new BinaryOperationNode(
            new ValueNode(new Token(TokenGroup.VALUE, TokenType.INT, (Int64)1)),
            new Token(TokenGroup.OPERATORS, TokenType.PLUS),
            new ValueNode(new Token(TokenGroup.VALUE, TokenType.INT, (Int64)1))
        );

        // Act
        var logger = A.Fake<ILogger>();
        var interpreterFactory = new InterpreterFactory(new StandardLibraryCallManager(logger), logger);
        var interpreter = new BinaryOperationInterpreter(binaryOperationNode, interpreterFactory, logger);
        var result = interpreter.VisitNode();

        // Assert
        Assert.IsType<List<BaseValue>>(result);
        Assert.IsType<Values.IntegerValue>(result[0]);
        Assert.Equal((Int64)2, result[0].Value);
    }

    [Fact]
    public void ShouldInterpretComparisonOperationNode()
    {
        // Arrange
        var comparisonOperationNode = new ComparisonOperationNode(
            new ValueNode(new Token(TokenGroup.VALUE, TokenType.INT, 1)),
            new Token(TokenGroup.COMPARISONOPERATORS, TokenType.DOUBLEEQUALS),
            new ValueNode(new Token(TokenGroup.VALUE, TokenType.INT, 1))
        );

        // Act
        var logger = A.Fake<ILogger>();
        var interpreterFactory = new InterpreterFactory(new StandardLibraryCallManager(logger), logger);
        var interpreter = new ComparisonOperationInterpreter(comparisonOperationNode, interpreterFactory, A.Fake<ILogger>());
        var result = interpreter.VisitNode();

        // Assert
        Assert.IsType<List<BaseValue>>(result);
        Assert.IsType<Values.BooleanValue>(result[0]);
        Assert.Equal(1, result[0].Value);
    }

    [Fact]
    public void ShouldInterpretValueNode()
    {
        // Arrange
        var valueNode = new ValueNode(new Token(TokenGroup.VALUE, TokenType.INT, 1));

        // Act
        var logger = A.Fake<ILogger>();
        var interpreterFactory = new InterpreterFactory(new StandardLibraryCallManager(logger), logger);
        var interpreter = new ValueInterpreter(valueNode, interpreterFactory, A.Fake<ILogger>());
        var result = interpreter.VisitNode();

        // Assert
        Assert.IsType<List<BaseValue>>(result);
        Assert.IsType<Values.IntegerValue>(result[0]);
        Assert.Equal(1, result[0].Value);
    }

    [Fact]
    public void ShouldInterpretVariableAssignNode()
    {
        // Arrange
        var variableAssignNode = new VariableDeclarationNode(
            new Token(TokenGroup.TYPEKEYWORD, TokenType.INT),
            new ValueNode(new Token(TokenGroup.SYNTAX, TokenType.IDENTIFIER, "a")),
            new ValueNode(new Token(TokenGroup.VALUE, TokenType.INT, 1))
        );

        // Act
        var logger = A.Fake<ILogger>();
        var interpreterFactory = new InterpreterFactory(new StandardLibraryCallManager(logger), logger);
        var interpreter = new VariableDeclarationInterpreter(variableAssignNode, interpreterFactory, A.Fake<ILogger>());
        var result = interpreter.VisitNode();

        // Assert
        Assert.IsType<List<BaseValue>>(result);
        Assert.IsType<Values.VariableValue>(result[0]);
        Assert.Equal(1, result[0].Value);
    }

    [Fact]
    public void ShouldInterpretVariableAssignNodeWithBinaryOperation()
    {
        // Arrange
        var variableAssignNode = new VariableDeclarationNode(
            new Token(TokenGroup.TYPEKEYWORD, TokenType.INT),
            new ValueNode(new Token(TokenGroup.SYNTAX, TokenType.IDENTIFIER, "a")),
            new BinaryOperationNode(
                new ValueNode(new Token(TokenGroup.VALUE, TokenType.INT, (Int64)1)),
                new Token(TokenGroup.OPERATORS, TokenType.PLUS),
                new ValueNode(new Token(TokenGroup.VALUE, TokenType.INT, (Int64)1))
            )
        );

        // Act
        var logger = A.Fake<ILogger>();
        var interpreterFactory = new InterpreterFactory(new StandardLibraryCallManager(logger), logger);
        var interpreter = new VariableDeclarationInterpreter(variableAssignNode, interpreterFactory, A.Fake<ILogger>());
        var result = interpreter.VisitNode();

        // Assert
        Assert.IsType<List<BaseValue>>(result);
        Assert.IsType<Values.VariableValue>(result[0]);
        Assert.Equal((Int64)2, result[0].Value);
    }

    [Fact]
    public void ShouldInterpretVariableAssignNodeWithComparisonOperation()
    {
        // Arrange
        var variableAssignNode = new VariableDeclarationNode(
            new Token(TokenGroup.TYPEKEYWORD, TokenType.VAR),
            new ValueNode(new Token(TokenGroup.SYNTAX, TokenType.IDENTIFIER, "a")),
            new ComparisonOperationNode(
                new ValueNode(new Token(TokenGroup.VALUE, TokenType.INT, 1)),
                new Token(TokenGroup.COMPARISONOPERATORS, TokenType.DOUBLEEQUALS),
                new ValueNode(new Token(TokenGroup.VALUE, TokenType.INT, 1))
            )
        );

        // Act
        var logger = A.Fake<ILogger>();
        var interpreterFactory = new InterpreterFactory(new StandardLibraryCallManager(logger), logger);
        var interpreter = new VariableDeclarationInterpreter(variableAssignNode, interpreterFactory, A.Fake<ILogger>());
        var result = interpreter.VisitNode();

        // Assert
        Assert.IsType<List<BaseValue>>(result);
        Assert.IsType<Values.VariableValue>(result[0]);
        Assert.Equal(1, result[0].Value);
    }

    [Fact]
    public void ShouldInterpretIfStatementNode()
    {
        // Arrange
        var ifStatementNode = new IfStatementNode(
            new ComparisonOperationNode(
                new ValueNode(new Token(TokenGroup.VALUE, TokenType.INT, 1)),
                new Token(TokenGroup.COMPARISONOPERATORS, TokenType.DOUBLEEQUALS),
                new ValueNode(new Token(TokenGroup.VALUE, TokenType.INT, 1))
            ),
            new List<INode>() {
                    new VariableDeclarationNode(
                        new Token(TokenGroup.TYPEKEYWORD, TokenType.VAR),
                        new ValueNode(new Token(TokenGroup.SYNTAX, TokenType.IDENTIFIER, "a")),
                        new ValueNode(new Token(TokenGroup.VALUE, TokenType.INT, 1))
                    )
            }
        );

        // Act
        var logger = A.Fake<ILogger>();
        var interpreterFactory = new InterpreterFactory(new StandardLibraryCallManager(logger), logger);
        var interpreter = new IfStatementInterpreter(ifStatementNode, interpreterFactory, A.Fake<ILogger>());
        var result = interpreter.VisitNode();

        // Assert
        Assert.IsType<List<BaseValue>>(result);
        Assert.IsType<Values.VariableValue>(result[0]);
        Assert.Equal(1, result[0].Value);
    }

    [Fact]
    public void ShouldInterpretIfStatementNodeWithElse()
    {
        // Arrange
        var ifStatementNode = new IfStatementNode(
            new ComparisonOperationNode(
                new ValueNode(new Token(TokenGroup.VALUE, TokenType.INT, 1)),
                new Token(TokenGroup.COMPARISONOPERATORS, TokenType.DOUBLEEQUALS),
                new ValueNode(new Token(TokenGroup.VALUE, TokenType.INT, 2))
            ),
            new List<INode>() {
                    new VariableDeclarationNode(
                        new Token(TokenGroup.TYPEKEYWORD, TokenType.VAR),
                        new ValueNode(new Token(TokenGroup.SYNTAX, TokenType.IDENTIFIER, "a")),
                        new ValueNode(new Token(TokenGroup.VALUE, TokenType.INT, 1))
                    )
            },
            new List<INode>() {
                    new VariableDeclarationNode(
                        new Token(TokenGroup.TYPEKEYWORD, TokenType.VAR),
                        new ValueNode(new Token(TokenGroup.SYNTAX, TokenType.IDENTIFIER, "a")),
                        new ValueNode(new Token(TokenGroup.VALUE, TokenType.INT, 2))
                    )
            }
        );

        // Act
        var logger = A.Fake<ILogger>();
        var interpreterFactory = new InterpreterFactory(new StandardLibraryCallManager(logger), logger);
        var interpreter = new IfStatementInterpreter(ifStatementNode, interpreterFactory, A.Fake<ILogger>());
        var result = interpreter.VisitNode();

        // Assert
        Assert.IsType<List<BaseValue>>(result);
        Assert.IsType<Values.VariableValue>(result[0]);
        Assert.Equal(2, result[0].Value);
    }

    // [Fact]
    // public void ShouldInterpretWhileLoopStatementNode()
    // {
    //     // Arrange
    //     var whileStatementNode = new WhileLoopStatementNode(
    //         new ComparisonOperationNode(
    //             new ValueNode(new Token(TokenGroup.VALUE, TokenType.INTVALUE, 1)),
    //             new Token(TokenGroup.COMPARISONOPERATORS, TokenType.DOUBLEEQUALS),
    //             new ValueNode(new Token(TokenGroup.VALUE, TokenType.INTVALUE, 1))
    //         ),
    //         new List<INode>() {
    //             new VariableAssignNode(
    //                 new Token(TokenGroup.TYPEKEYWORD, TokenType.VAR),
    //                 new ValueNode(new Token(TokenGroup.SYNTAX, TokenType.IDENTIFIER, "a")),
    //                 new ValueNode(new Token(TokenGroup.VALUE, TokenType.INTVALUE, 1))
    //             )
    //         }
    //     );

    //     // Act
    //     var interpreter = new WhileLoopStatementNodeInterpreter(whileStatementNode, interpreterFactory, A.Fake<ILogger>());
    //     var result = interpreter.VisitNode();

    //     // Assert
    //     Assert.IsType<Values.Variable>(result);
    //     Assert.Equal(1, result[0].Value);
    // }

    [Fact]
    public void ShouldInterpretForLoopStatementNode()
    {
        // Arrange
        var forStatementNode = new ForLoopStatementNode(
            new VariableDeclarationNode(
                new Token(TokenGroup.TYPEKEYWORD, TokenType.VAR),
                new ValueNode(new Token(TokenGroup.SYNTAX, TokenType.IDENTIFIER, "a")),
                new ValueNode(new Token(TokenGroup.VALUE, TokenType.INT, 0))
            ),
            new ValueNode(new Token(TokenGroup.VALUE, TokenType.INT, 10)),
            new List<INode>() {
                    new VariableDeclarationNode(
                        new Token(TokenGroup.TYPEKEYWORD, TokenType.VAR),
                        new ValueNode(new Token(TokenGroup.SYNTAX, TokenType.IDENTIFIER, "a")),
                        new ValueNode(new Token(TokenGroup.VALUE, TokenType.INT, 1))
                    )
            }
        );

        // Act
        var logger = A.Fake<ILogger>();
        var interpreterFactory = new InterpreterFactory(new StandardLibraryCallManager(logger), logger);
        var interpreter = new ForLoopStatementInterpreter(forStatementNode, interpreterFactory, A.Fake<ILogger>());
        var result = interpreter.VisitNode();

        // Assert
        Assert.IsType<List<BaseValue>>(result);
        Assert.IsType<Values.VariableValue>(result[0]);
        Assert.Equal(1, result[0].Value);
    }

    [Fact]
    public void ShouldInterpretFunctionDeclarationNode()
    {
        // Arrange
        var functionDeclarationNode = new FunctionDeclarationNode(
            new ValueNode(new Token(TokenGroup.SYNTAX, TokenType.IDENTIFIER, "a")),
            new List<IParameterDefinitionNode>() {
                new ParameterDefinitionNode(
                    new Token(TokenGroup.TYPEKEYWORD, TokenType.VAR),
                    new ValueNode(new Token(TokenGroup.SYNTAX, TokenType.IDENTIFIER, "a"))
                )
            },
            new Token(TokenGroup.TYPEKEYWORD, TokenType.VOID),
            new List<INode>() {
                new VariableDeclarationNode(
                    new Token(TokenGroup.TYPEKEYWORD, TokenType.VAR),
                    new ValueNode(new Token(TokenGroup.SYNTAX, TokenType.IDENTIFIER, "a")),
                    new ValueNode(new Token(TokenGroup.VALUE, TokenType.INT, 1))
                )
            },
            new ValueNode(new Token(TokenGroup.VALUE, TokenType.INT, 1))
        );

        // Act
        var logger = A.Fake<ILogger>();
        var interpreterFactory = new InterpreterFactory(new StandardLibraryCallManager(logger), logger);
        var interpreter = new FunctionDeclarationInterpreter(functionDeclarationNode, interpreterFactory, A.Fake<ILogger>());
        var result = interpreter.VisitNode();

        // Assert
        Assert.IsType<List<BaseValue>>(result);
        Assert.True(result.Count == 0);
        
    }

    [Fact]
    public void ShouldInterpretFunctionCallNode()
    {
        // Arrange
        var functionCallNode = new FunctionCallNode(
            new ValueNode(new Token(TokenGroup.SYNTAX, TokenType.IDENTIFIER, "a")),
            new List<INode>() {
                new ValueNode(new Token(TokenGroup.VALUE, TokenType.INT, 1))
            }
        );

        SymbolTable.Instance(A.Fake<ILogger>()).SetBaseValue("a", new FunctionValue(new FunctionDeclarationNode(
            new ValueNode(new Token(TokenGroup.SYNTAX, TokenType.IDENTIFIER, "a")),
            new List<IParameterDefinitionNode>() {
                new ParameterDefinitionNode(
                    new Token(TokenGroup.TYPEKEYWORD, TokenType.VAR),
                    new ValueNode(new Token(TokenGroup.SYNTAX, TokenType.IDENTIFIER, "a"))
                )
            },
            new Token(TokenGroup.TYPEKEYWORD, TokenType.VOID),
            new List<INode>() {
                new VariableDeclarationNode(
                    new Token(TokenGroup.TYPEKEYWORD, TokenType.VAR),
                    new ValueNode(new Token(TokenGroup.SYNTAX, TokenType.IDENTIFIER, "a")),
                    new ValueNode(new Token(TokenGroup.VALUE, TokenType.INT, 1))
                )
            },
            new ValueNode(new Token(TokenGroup.VALUE, TokenType.INT, 1))
        ), A.Fake<ILogger>()));

        // Act
        var logger = A.Fake<ILogger>();
        var interpreterFactory = new InterpreterFactory(new StandardLibraryCallManager(logger), logger);
        var interpreter = new FunctionCallInterpreter(functionCallNode, interpreterFactory, A.Fake<ILogger>(), A.Fake<IStandardLibraryCallManager>());
        var result = interpreter.VisitNode();

        // Assert
        Assert.IsType<List<BaseValue>>(result);
    }

    [Fact]
    public void ShouldInterpretVariableDeclarationNode()
    {
        // Arrange
        var variableDeclarationNode = new VariableDeclarationNode(
            new Token(TokenGroup.TYPEKEYWORD, TokenType.VAR),
            new ValueNode(new Token(TokenGroup.SYNTAX, TokenType.IDENTIFIER, "a")),
            new ValueNode(new Token(TokenGroup.VALUE, TokenType.INT, 1))
        );

        // Act
        var logger = A.Fake<ILogger>();
        var interpreterFactory = new InterpreterFactory(new StandardLibraryCallManager(logger), logger);
        var interpreter = new VariableDeclarationInterpreter(variableDeclarationNode, interpreterFactory, A.Fake<ILogger>());
        var result = interpreter.VisitNode();

        // Assert
        Assert.IsType<List<BaseValue>>(result);
        Assert.True(result.Count == 1);
    }
}
