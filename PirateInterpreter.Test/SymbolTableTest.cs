using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PirateInterpreter.Interpreters;
using PirateInterpreter.Values;
using PirateParser.Node;
using Xunit;

namespace PirateInterpreter.Test
{
    public class SymbolTableTest
    {
        [Fact]
        public void ShouldSetValue()
        {
            // Arrange
            var symbolTable = SymbolTable.Instance(A.Fake<ILogger>());
            var value = new ValueNode(new Token(TokenGroup.VALUE, TokenValue.INT, "1"));

            // Act
            var result = symbolTable.Set("test", value);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ShouldGetValue()
        {
            // Arrange
            var symbolTable = SymbolTable.Instance(A.Fake<ILogger>());
            var value = new ValueNode(new Token(TokenGroup.VALUE, TokenValue.INT, "1"));
            symbolTable.Set("test", value);

            // Act
            var result = symbolTable.Get("test");

            // Assert
            Assert.Equal(value, result);
        }

        public void ShouldRemoveValue()
        {
            // Arrange
            var symbolTable = SymbolTable.Instance(A.Fake<ILogger>());
            var value = new ValueNode(new Token(TokenGroup.VALUE, TokenValue.INT, "1"));
            symbolTable.Set("test", value);

            // Act
            var result = symbolTable.Remove("test");

            // Assert
            Assert.True(result);
        }
    }
}