namespace Pirate.Interpreter.Test
{
    public class SymbolTableTest
    {
        [Fact]
        public void ShouldSetValue()
        {
            // Arrange
            var symbolTable = SymbolTable.Instance(A.Fake<ILogger>());
            var value = new Values.StringValue("test", A.Fake<ILogger>());

            // Act
            var result = symbolTable.SetBaseValue("test", value);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ShouldGetValue()
        {
            // Arrange
            var symbolTable = SymbolTable.Instance(A.Fake<ILogger>());
            var value = new Values.StringValue("test", A.Fake<ILogger>());
            symbolTable.SetBaseValue("test", value);

            // Act
            var result = symbolTable.GetBaseValue("test");

            // Assert
            Assert.Equal(value, result);
        }

        [Fact]
        public void ShouldRemoveValue()
        {
            // Arrange
            var symbolTable = SymbolTable.Instance(A.Fake<ILogger>());
            var value = new Values.StringValue("test", A.Fake<ILogger>());
            symbolTable.SetBaseValue("test", value);

            // Act
            var result = symbolTable.Remove("test");

            // Assert
            Assert.True(result);
        }
    }
}