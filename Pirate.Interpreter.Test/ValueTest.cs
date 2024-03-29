using System;
using Pirate.Interpreter.Values;

namespace Pirate.Interpreter.Test
{
    public class ValueTest
    {
        [Fact]
        public void ShouldOperateBooleanPlusBoolean()
        {
            // Arrange
            var boolean1 = new BooleanValue((long)1, A.Fake<ILogger>());
            var boolean2 = new BooleanValue((long)1, A.Fake<ILogger>());
            var tokenOperator = new Token(TokenGroup.OPERATORS, TokenType.PLUS);
            // Act
            var result = boolean1.OperatedBy(tokenOperator, boolean2);

            // Assert
            Assert.IsType<IntegerValue>(result);
            Assert.Equal((long)2, result.Value);
        }

        [Fact]
        public void ShouldOperateBooleanMinusBoolean()
        {
            // Arrange
            var boolean1 = new BooleanValue((long)1, A.Fake<ILogger>());
            var boolean2 = new BooleanValue((long)1, A.Fake<ILogger>());
            var tokenOperator = new Token(TokenGroup.OPERATORS, TokenType.MINUS);
            // Act
            var result = boolean1.OperatedBy(tokenOperator, boolean2);

            // Assert
            Assert.IsType<IntegerValue>(result);
            Assert.Equal((long)0, result.Value);
        }

        [Fact]
        public void ShouldOperateBooleanMultiplyBoolean()
        {
            // Arrange
            var boolean1 = new BooleanValue((long)1, A.Fake<ILogger>());
            var boolean2 = new BooleanValue((long)1, A.Fake<ILogger>());
            var tokenOperator = new Token(TokenGroup.OPERATORS, TokenType.MULTIPLY);
            // Act
            var result = boolean1.OperatedBy(tokenOperator, boolean2);

            // Assert
            Assert.IsType<IntegerValue>(result);
            Assert.Equal((long)1, result.Value);
        }

        [Fact]
        public void ShouldOperateBooleanDivideBoolean()
        {
            // Arrange
            var boolean1 = new BooleanValue((long)1, A.Fake<ILogger>());
            var boolean2 = new BooleanValue((long)1, A.Fake<ILogger>());
            var tokenOperator = new Token(TokenGroup.OPERATORS, TokenType.DIVIDE);
            // Act
            var result = boolean1.OperatedBy(tokenOperator, boolean2);

            // Assert
            Assert.IsType<IntegerValue>(result);
            Assert.Equal((long)1, result.Value);
        }

        [Fact]
        public void ShouldOperateBooleanPowerBoolean()
        {
            // Arrange
            var boolean1 = new BooleanValue((long)1, A.Fake<ILogger>());
            var boolean2 = new BooleanValue((long)1, A.Fake<ILogger>());
            var tokenOperator = new Token(TokenGroup.OPERATORS, TokenType.POWER);
            // Act
            var result = boolean1.OperatedBy(tokenOperator, boolean2);

            // Assert
            Assert.IsType<IntegerValue>(result);
            Assert.Equal((long)1, result.Value);
        }

        [Fact]
        public void ShouldOperateIntegerPlusInteger()
        {
            // Arrange
            var integer1 = new IntegerValue((long)1, A.Fake<ILogger>());
            var integer2 = new IntegerValue((long)1, A.Fake<ILogger>());
            var tokenOperator = new Token(TokenGroup.OPERATORS, TokenType.PLUS);
            // Act
            var result = integer1.OperatedBy(tokenOperator, integer2);

            // Assert
            Assert.IsType<IntegerValue>(result);
            Assert.Equal((long)2, result.Value);
        }

        [Fact]
        public void ShouldOperateIntegerMinusInteger()
        {
            // Arrange
            var integer1 = new IntegerValue((long)1, A.Fake<ILogger>());
            var integer2 = new IntegerValue((long)1, A.Fake<ILogger>());
            var tokenOperator = new Token(TokenGroup.OPERATORS, TokenType.MINUS);
            // Act
            var result = integer1.OperatedBy(tokenOperator, integer2);

            // Assert
            Assert.IsType<IntegerValue>(result);
            Assert.Equal((long)0, result.Value);
        }

        [Fact]
        public void ShouldOperateIntegerMultiplyInteger()
        {
            // Arrange
            var integer1 = new IntegerValue((long)1, A.Fake<ILogger>());
            var integer2 = new IntegerValue((long)1, A.Fake<ILogger>());
            var tokenOperator = new Token(TokenGroup.OPERATORS, TokenType.MULTIPLY);
            // Act
            var result = integer1.OperatedBy(tokenOperator, integer2);

            // Assert
            Assert.IsType<IntegerValue>(result);
            Assert.Equal((long)1, result.Value);
        }

        [Fact]
        public void ShouldOperateIntegerDivideInteger()
        {
            // Arrange
            var integer1 = new IntegerValue((long)1, A.Fake<ILogger>());
            var integer2 = new IntegerValue((long)1, A.Fake<ILogger>());
            var tokenOperator = new Token(TokenGroup.OPERATORS, TokenType.DIVIDE);
            // Act
            var result = integer1.OperatedBy(tokenOperator, integer2);

            // Assert
            Assert.IsType<IntegerValue>(result);
            Assert.Equal((long)1, result.Value);
        }

        [Fact]
        public void ShouldOperateIntegerPowerInteger()
        {
            // Arrange
            var integer1 = new IntegerValue((long)1, A.Fake<ILogger>());
            var integer2 = new IntegerValue((long)1, A.Fake<ILogger>());
            var tokenOperator = new Token(TokenGroup.OPERATORS, TokenType.POWER);
            // Act
            var result = integer1.OperatedBy(tokenOperator, integer2);

            // Assert
            Assert.IsType<IntegerValue>(result);
            Assert.Equal((long)1, result.Value);
        }

        [Fact]
        public void ShouldOperateStringPlusString()
        {
            // Arrange
            var string1 = new StringValue("1", A.Fake<ILogger>());
            var string2 = new StringValue("1", A.Fake<ILogger>());
            var tokenOperator = new Token(TokenGroup.OPERATORS, TokenType.PLUS);
            // Act
            var result = string1.OperatedBy(tokenOperator, string2);

            // Assert
            Assert.IsType<StringValue>(result);
            Assert.Equal("11", result.Value);
        }

        [Fact]
        public void ShouldOperateStringMultiplyInteger()
        {
            // Arrange
            var string1 = new StringValue("1", A.Fake<ILogger>());
            var integer2 = new IntegerValue(1, A.Fake<ILogger>());
            var tokenOperator = new Token(TokenGroup.OPERATORS, TokenType.MULTIPLY);
            // Act
            var result = string1.OperatedBy(tokenOperator, integer2);

            // Assert
            Assert.IsType<StringValue>(result);
            Assert.Equal("1", result.Value);
        }

        [Fact]
        public void ShouldOperateFloatPlusFloat()
        {
            // Arrange
            var float1 = new FloatValue(1.1, A.Fake<ILogger>());
            var float2 = new FloatValue(1.1, A.Fake<ILogger>());
            var tokenOperator = new Token(TokenGroup.OPERATORS, TokenType.PLUS);
            // Act
            var result = float1.OperatedBy(tokenOperator, float2);

            // Assert
            Assert.IsType<FloatValue>(result);
            Assert.Equal(2.2, result.Value);
        }

        [Fact]
        public void ShouldOperateFloatMinusFloat()
        {
            // Arrange
            var float1 = new FloatValue(1.1, A.Fake<ILogger>());
            var float2 = new FloatValue(1.1, A.Fake<ILogger>());
            var tokenOperator = new Token(TokenGroup.OPERATORS, TokenType.MINUS);
            // Act
            var result = float1.OperatedBy(tokenOperator, float2);

            // Assert
            Assert.IsType<FloatValue>(result);
            Assert.Equal((double)0, result.Value);
        }

        [Fact]
        public void ShouldOperateFloatMultiplyFloat()
        {
            // Arrange
            var float1 = new FloatValue(1.1, A.Fake<ILogger>());
            var float2 = new FloatValue(1.1, A.Fake<ILogger>());
            var tokenOperator = new Token(TokenGroup.OPERATORS, TokenType.MULTIPLY);
            // Act
            var result = float1.OperatedBy(tokenOperator, float2);

            // Assert
            Assert.IsType<FloatValue>(result);
            Assert.Equal((double)1.21, Math.Round((double)result.Value, 2));
        }

        [Fact]
        public void ShouldOperateFloatDivideFloat()
        {
            // Arrange
            var float1 = new FloatValue(1.1, A.Fake<ILogger>());
            var float2 = new FloatValue(1.1, A.Fake<ILogger>());
            var tokenOperator = new Token(TokenGroup.OPERATORS, TokenType.DIVIDE);
            // Act
            var result = float1.OperatedBy(tokenOperator, float2);

            // Assert
            Assert.IsType<FloatValue>(result);
            Assert.Equal((double)1, result.Value);
        }

        [Fact]
        public void ShouldOperateFloatPowerFloat()
        {
            // Arrange
            var float1 = new FloatValue(1.1, A.Fake<ILogger>());
            var float2 = new FloatValue(1.1, A.Fake<ILogger>());
            var tokenOperator = new Token(TokenGroup.OPERATORS, TokenType.POWER);
            // Act
            var result = float1.OperatedBy(tokenOperator, float2);

            // Assert
            Assert.Equal((double)1.11053, Math.Round((double)result.Value, 5));
        }
    }
}