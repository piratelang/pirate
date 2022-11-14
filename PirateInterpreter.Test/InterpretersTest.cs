using System.Collections.Generic;
using PirateInterpreter.Interpreters;
using PirateInterpreter.Values;
using PirateParser.Node;
using PirateParser.Node.Interfaces;

namespace PirateInterpreter.Test
{
    public class InterpretersTest
    {
        [Fact]
        public void ShouldInterpretBinaryOperationNode()
        {
            // Arrange
            var binaryOperationNode = new BinaryOperationNode(
                new ValueNode(new Token(TokenGroup.VALUE, TokenValue.INT, 1)),
                new Token(TokenGroup.OPERATORS, TokenOperators.PLUS),
                new ValueNode(new Token(TokenGroup.VALUE, TokenValue.INT, 1))
            );

            // Act
            var interpreter = new BinaryOperationNodeInterpreter(binaryOperationNode, new InterpreterFactory() ,A.Fake<ILogger>());
            var result = interpreter.VisitNode();

            // Assert
            Assert.IsType<List<BaseValue>>(result);
            Assert.IsType<Values.Integer>(result[0]);
            Assert.Equal(2, result[0].Value);
        }

        [Fact]
        public void ShouldInterpretComparisonOperationNode()
        {
            // Arrange
            var comparisonOperationNode = new ComparisonOperationNode(
                new ValueNode(new Token(TokenGroup.VALUE, TokenValue.INT, 1)),
                new Token(TokenGroup.COMPARISONOPERATORS, TokenComparisonOperators.DOUBLEEQUALS),
                new ValueNode(new Token(TokenGroup.VALUE, TokenValue.INT, 1))
            );

            // Act
            var interpreter = new ComparisonOperationNodeInterpreter(comparisonOperationNode, new InterpreterFactory(), A.Fake<ILogger>());
            var result = interpreter.VisitNode();

            // Assert
            Assert.IsType<List<BaseValue>>(result);
            Assert.IsType<Values.Boolean>(result[0]);
            Assert.Equal(1, result[0].Value);
        }

        [Fact]
        public void ShouldInterpretValueNode()
        {
            // Arrange
            var valueNode = new ValueNode(new Token(TokenGroup.VALUE, TokenValue.INT, 1));

            // Act
            var interpreter = new ValueNodeInterpreter(valueNode, new InterpreterFactory(), A.Fake<ILogger>());
            var result = interpreter.VisitNode();

            // Assert
            Assert.IsType<List<BaseValue>>(result);
            Assert.IsType<Values.Integer>(result[0]);
            Assert.Equal(1, result[0].Value);
        }

        [Fact]
        public void ShouldInterpretVariableAssignNode()
        {
            // Arrange
            var variableAssignNode = new VariableAssignNode(
                new Token(TokenGroup.TYPEKEYWORD, TokenTypeKeyword.INT),
                new ValueNode(new Token(TokenGroup.SYNTAX, TokenSyntax.IDENTIFIER, "a")),
                new ValueNode(new Token(TokenGroup.VALUE, TokenValue.INT, 1))
            );

            // Act
            var interpreter = new VariableAssignNodeInterpreter(variableAssignNode, new InterpreterFactory(), A.Fake<ILogger>());
            var result = interpreter.VisitNode();

            // Assert
            Assert.IsType<List<BaseValue>>(result);
            Assert.IsType<Values.Variable>(result[0]);
            Assert.Equal(1, result[0].Value);
        }

        [Fact]
        public void ShouldInterpretVariableAssignNodeWithBinaryOperation()
        {
            // Arrange
            var variableAssignNode = new VariableAssignNode(
                new Token(TokenGroup.TYPEKEYWORD, TokenTypeKeyword.INT),
                new ValueNode(new Token(TokenGroup.SYNTAX, TokenSyntax.IDENTIFIER, "a")),
                new BinaryOperationNode(
                    new ValueNode(new Token(TokenGroup.VALUE, TokenValue.INT, 1)),
                    new Token(TokenGroup.OPERATORS, TokenOperators.PLUS),
                    new ValueNode(new Token(TokenGroup.VALUE, TokenValue.INT, 1))
                )
            );

            // Act
            var interpreter = new VariableAssignNodeInterpreter(variableAssignNode, new InterpreterFactory(), A.Fake<ILogger>());
            var result = interpreter.VisitNode();

            // Assert
            Assert.IsType<List<BaseValue>>(result);
            Assert.IsType<Values.Variable>(result[0]);
            Assert.Equal(2, result[0].Value);
        }

        [Fact]
        public void ShouldInterpretVariableAssignNodeWithComparisonOperation()
        {
            // Arrange
            var variableAssignNode = new VariableAssignNode(
                new Token(TokenGroup.TYPEKEYWORD, TokenTypeKeyword.VAR),
                new ValueNode(new Token(TokenGroup.SYNTAX, TokenSyntax.IDENTIFIER, "a")),
                new ComparisonOperationNode(
                    new ValueNode(new Token(TokenGroup.VALUE, TokenValue.INT, 1)),
                    new Token(TokenGroup.COMPARISONOPERATORS, TokenComparisonOperators.DOUBLEEQUALS),
                    new ValueNode(new Token(TokenGroup.VALUE, TokenValue.INT, 1))
                )
            );

            // Act
            var interpreter = new VariableAssignNodeInterpreter(variableAssignNode, new InterpreterFactory(), A.Fake<ILogger>());
            var result = interpreter.VisitNode();

            // Assert
            Assert.IsType<List<BaseValue>>(result);
            Assert.IsType<Values.Variable>(result[0]);
            Assert.Equal(1, result[0].Value);
        }

        [Fact]
        public void ShouldInterpretIfStatementNode()
        {
            // Arrange
            var ifStatementNode = new IfStatementNode(
                new ComparisonOperationNode(
                    new ValueNode(new Token(TokenGroup.VALUE, TokenValue.INT, 1)),
                    new Token(TokenGroup.COMPARISONOPERATORS, TokenComparisonOperators.DOUBLEEQUALS),
                    new ValueNode(new Token(TokenGroup.VALUE, TokenValue.INT, 1))
                ),
                new List<INode>() {
                    new VariableAssignNode(
                        new Token(TokenGroup.TYPEKEYWORD, TokenTypeKeyword.VAR),
                        new ValueNode(new Token(TokenGroup.SYNTAX, TokenSyntax.IDENTIFIER, "a")),
                        new ValueNode(new Token(TokenGroup.VALUE, TokenValue.INT, 1))
                    )
                }
            );

            // Act
            var interpreter = new IfStatementNodeInterpreter(ifStatementNode, new InterpreterFactory(), A.Fake<ILogger>());
            var result = interpreter.VisitNode();

            // Assert
            Assert.IsType<List<BaseValue>>(result);
            Assert.IsType<Values.Variable>(result[0]);
            Assert.Equal(1, result[0].Value);
        }

        [Fact]
        public void ShouldInterpretIfStatementNodeWithElse()
        {
            // Arrange
            var ifStatementNode = new IfStatementNode(
                new ComparisonOperationNode(
                    new ValueNode(new Token(TokenGroup.VALUE, TokenValue.INT, 1)),
                    new Token(TokenGroup.COMPARISONOPERATORS, TokenComparisonOperators.DOUBLEEQUALS),
                    new ValueNode(new Token(TokenGroup.VALUE, TokenValue.INT, 2))
                ),
                new List<INode>() {
                    new VariableAssignNode(
                        new Token(TokenGroup.TYPEKEYWORD, TokenTypeKeyword.VAR),
                        new ValueNode(new Token(TokenGroup.SYNTAX, TokenSyntax.IDENTIFIER, "a")),
                        new ValueNode(new Token(TokenGroup.VALUE, TokenValue.INT, 1))
                    )
                },
                new List<INode>() {
                    new VariableAssignNode(
                        new Token(TokenGroup.TYPEKEYWORD, TokenTypeKeyword.VAR),
                        new ValueNode(new Token(TokenGroup.SYNTAX, TokenSyntax.IDENTIFIER, "a")),
                        new ValueNode(new Token(TokenGroup.VALUE, TokenValue.INT, 2))
                    )
                }
            );

            // Act
            var interpreter = new IfStatementNodeInterpreter(ifStatementNode, new InterpreterFactory(), A.Fake<ILogger>());
            var result = interpreter.VisitNode();

            // Assert
            Assert.IsType<List<BaseValue>>(result);
            Assert.IsType<Values.Variable>(result[0]);
            Assert.Equal(2, result[0].Value);
        }

        // [Fact]
        // public void ShouldInterpretWhileLoopStatementNode()
        // {
        //     // Arrange
        //     var whileStatementNode = new WhileLoopStatementNode(
        //         new ComparisonOperationNode(
        //             new ValueNode(new Token(TokenGroup.VALUE, TokenValue.INT, 1)),
        //             new Token(TokenGroup.COMPARISONOPERATORS, TokenComparisonOperators.DOUBLEEQUALS),
        //             new ValueNode(new Token(TokenGroup.VALUE, TokenValue.INT, 1))
        //         ),
        //         new List<INode>() {
        //             new VariableAssignNode(
        //                 new Token(TokenGroup.TYPEKEYWORD, TokenTypeKeyword.VAR),
        //                 new ValueNode(new Token(TokenGroup.SYNTAX, TokenSyntax.IDENTIFIER, "a")),
        //                 new ValueNode(new Token(TokenGroup.VALUE, TokenValue.INT, 1))
        //             )
        //         }
        //     );

        //     // Act
        //     var interpreter = new WhileLoopStatementNodeInterpreter(whileStatementNode, new InterpreterFactory(), A.Fake<ILogger>());
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
                new VariableAssignNode(
                    new Token(TokenGroup.TYPEKEYWORD, TokenTypeKeyword.VAR),
                    new ValueNode(new Token(TokenGroup.SYNTAX, TokenSyntax.IDENTIFIER, "a")),
                    new ValueNode(new Token(TokenGroup.VALUE, TokenValue.INT, 0))
                ),
                new ValueNode(new Token(TokenGroup.VALUE, TokenValue.INT, 10)),
                new List<INode>() {
                    new VariableAssignNode(
                        new Token(TokenGroup.TYPEKEYWORD, TokenTypeKeyword.VAR),
                        new ValueNode(new Token(TokenGroup.SYNTAX, TokenSyntax.IDENTIFIER, "a")),
                        new ValueNode(new Token(TokenGroup.VALUE, TokenValue.INT, 1))
                    )
                }
            );

            // Act
            var interpreter = new ForLoopStatementNodeInterpreter(forStatementNode, new InterpreterFactory(), A.Fake<ILogger>());
            var result = interpreter.VisitNode();

            // Assert
            Assert.IsType<List<BaseValue>>(result);
            Assert.IsType<Values.Variable>(result[0]);
            Assert.Equal(1, result[0].Value);
        }
    }
}