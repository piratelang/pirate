using PirateParser.Node;

namespace PirateInterpreter.Test
{
    public class SymbolTableTest
    {
        [Fact]
        public void ShouldSetValue()
        {
            // Arrange
            var symbolTable = SymbolTable.Instance(A.Fake<ILogger>());
            var value = new Values.String("test", A.Fake<ILogger>());

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
            var value = new Values.String("test", A.Fake<ILogger>());
            symbolTable.Set("test", value);

            // Act
            var result = symbolTable.Get("test");

            // Assert
            Assert.Equal(value, result);
        }

        [Fact]
        public void ShouldRemoveValue()
        {
            // Arrange
            var symbolTable = SymbolTable.Instance(A.Fake<ILogger>());
            var value = new Values.String("test", A.Fake<ILogger>());
            symbolTable.Set("test", value);

            // Act
            var result = symbolTable.Remove("test");

            // Assert
            Assert.True(result);
        }
    }
}