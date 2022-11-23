using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PirateInterpreter.Values;
using Xunit;

namespace PirateInterpreter.Test
{
    public class ValueTest
    {
        [Fact]
        public void ShouldOperateBooleanPlusBoolean()
        {
            // Arrange
            var boolean1 = new Values.BooleanValue(1, A.Fake<ILogger>());
            var boolean2 = new Values.BooleanValue(1, A.Fake<ILogger>());
            var tokenOperator = new Token(TokenGroup.OPERATORS, TokenType.PLUS);
            // Act
            var result = boolean1.OperatedBy(tokenOperator, boolean2);

            // Assert
            Assert.IsType<Values.IntegerValue>(result);
            Assert.Equal(2, result.Value);
        }

        [Fact]
        public void ShouldOperateBooleanMinusBoolean()
        {
            // Arrange
            var boolean1 = new Values.BooleanValue(1, A.Fake<ILogger>());
            var boolean2 = new Values.BooleanValue(1, A.Fake<ILogger>());
            var tokenOperator = new Token(TokenGroup.OPERATORS, TokenType.MINUS);
            // Act
            var result = boolean1.OperatedBy(tokenOperator, boolean2);

            // Assert
            Assert.IsType<Values.IntegerValue>(result);
            Assert.Equal(0, result.Value);
        }

        [Fact]
        public void ShouldOperateBooleanMultiplyBoolean()
        {
            // Arrange
            var boolean1 = new Values.BooleanValue(1, A.Fake<ILogger>());
            var boolean2 = new Values.BooleanValue(1, A.Fake<ILogger>());
            var tokenOperator = new Token(TokenGroup.OPERATORS, TokenType.MULTIPLY);
            // Act
            var result = boolean1.OperatedBy(tokenOperator, boolean2);

            // Assert
            Assert.IsType<Values.IntegerValue>(result);
            Assert.Equal(1, result.Value);
        }

        [Fact]
        public void ShouldOperateBooleanDivideBoolean()
        {
            // Arrange
            var boolean1 = new Values.BooleanValue(1, A.Fake<ILogger>());
            var boolean2 = new Values.BooleanValue(1, A.Fake<ILogger>());
            var tokenOperator = new Token(TokenGroup.OPERATORS, TokenType.DIVIDE);
            // Act
            var result = boolean1.OperatedBy(tokenOperator, boolean2);

            // Assert
            Assert.IsType<Values.IntegerValue>(result);
            Assert.Equal(1, result.Value);
        }

        [Fact]
        public void ShouldOperateBooleanPowerBoolean()
        {
            // Arrange
            var boolean1 = new Values.BooleanValue(1, A.Fake<ILogger>());
            var boolean2 = new Values.BooleanValue(1, A.Fake<ILogger>());
            var tokenOperator = new Token(TokenGroup.OPERATORS, TokenType.POWER);
            // Act
            var result = boolean1.OperatedBy(tokenOperator, boolean2);

            // Assert
            Assert.IsType<Values.IntegerValue>(result);
            Assert.Equal(1, result.Value);
        }

        [Fact]
        public void ShouldOperateIntegerPlusInteger()
        {
            // Arrange
            var integer1 = new Values.IntegerValue(1, A.Fake<ILogger>());
            var integer2 = new Values.IntegerValue(1, A.Fake<ILogger>());
            var tokenOperator = new Token(TokenGroup.OPERATORS, TokenType.PLUS);
            // Act
            var result = integer1.OperatedBy(tokenOperator, integer2);

            // Assert
            Assert.IsType<Values.IntegerValue>(result);
            Assert.Equal(2, result.Value);
        }

        [Fact]
        public void ShouldOperateIntegerMinusInteger()
        {
            // Arrange
            var integer1 = new Values.IntegerValue(1, A.Fake<ILogger>());
            var integer2 = new Values.IntegerValue(1, A.Fake<ILogger>());
            var tokenOperator = new Token(TokenGroup.OPERATORS, TokenType.MINUS);
            // Act
            var result = integer1.OperatedBy(tokenOperator, integer2);

            // Assert
            Assert.IsType<Values.IntegerValue>(result);
            Assert.Equal(0, result.Value);
        }

        [Fact]
        public void ShouldOperateIntegerMultiplyInteger()
        {
            // Arrange
            var integer1 = new Values.IntegerValue(1, A.Fake<ILogger>());
            var integer2 = new Values.IntegerValue(1, A.Fake<ILogger>());
            var tokenOperator = new Token(TokenGroup.OPERATORS, TokenType.MULTIPLY);
            // Act
            var result = integer1.OperatedBy(tokenOperator, integer2);

            // Assert
            Assert.IsType<Values.IntegerValue>(result);
            Assert.Equal(1, result.Value);
        }

        [Fact]
        public void ShouldOperateIntegerDivideInteger()
        {
            // Arrange
            var integer1 = new Values.IntegerValue(1, A.Fake<ILogger>());
            var integer2 = new Values.IntegerValue(1, A.Fake<ILogger>());
            var tokenOperator = new Token(TokenGroup.OPERATORS, TokenType.DIVIDE);
            // Act
            var result = integer1.OperatedBy(tokenOperator, integer2);

            // Assert
            Assert.IsType<Values.IntegerValue>(result);
            Assert.Equal(1, result.Value);
        }

        [Fact]
        public void ShouldOperateIntegerPowerInteger()
        {
            // Arrange
            var integer1 = new Values.IntegerValue(1, A.Fake<ILogger>());
            var integer2 = new Values.IntegerValue(1, A.Fake<ILogger>());
            var tokenOperator = new Token(TokenGroup.OPERATORS, TokenType.POWER);
            // Act
            var result = integer1.OperatedBy(tokenOperator, integer2);

            // Assert
            Assert.IsType<Values.IntegerValue>(result);
            Assert.Equal(1, result.Value);
        }

        [Fact]
        public void ShouldOperateStringPlusString()
        {
            // Arrange
            var string1 = new Values.StringValue("1", A.Fake<ILogger>());
            var string2 = new Values.StringValue("1", A.Fake<ILogger>());
            var tokenOperator = new Token(TokenGroup.OPERATORS, TokenType.PLUS);
            // Act
            var result = string1.OperatedBy(tokenOperator, string2);

            // Assert
            Assert.IsType<Values.StringValue>(result);
            Assert.Equal("11", result.Value);
        }

        [Fact]
        public void ShouldOperateStringMultiplyInteger()
        {
            // Arrange
            var string1 = new Values.StringValue("1", A.Fake<ILogger>());
            var integer2 = new Values.IntegerValue(1, A.Fake<ILogger>());
            var tokenOperator = new Token(TokenGroup.OPERATORS, TokenType.MULTIPLY);
            // Act
            var result = string1.OperatedBy(tokenOperator, integer2);

            // Assert
            Assert.IsType<Values.StringValue>(result);
            Assert.Equal("1", result.Value);
        }

        [Fact]
        public void ShouldOperateFloatPlusFloat()
        {
            // Arrange
            var float1 = new Values.FloatValue((float)1.1, A.Fake<ILogger>());
            var float2 = new Values.FloatValue((float)1.1, A.Fake<ILogger>());
            var tokenOperator = new Token(TokenGroup.OPERATORS, TokenType.PLUS);
            // Act
            var result = float1.OperatedBy(tokenOperator, float2);

            // Assert
            Assert.IsType<Values.FloatValue>(result);
            Assert.Equal((float)2.2, result.Value);
        }

        [Fact]
        public void ShouldOperateFloatMinusFloat()
        {
            // Arrange
            var float1 = new Values.FloatValue((float)1.1, A.Fake<ILogger>());
            var float2 = new Values.FloatValue((float)1.1, A.Fake<ILogger>());
            var tokenOperator = new Token(TokenGroup.OPERATORS, TokenType.MINUS);
            // Act
            var result = float1.OperatedBy(tokenOperator, float2);

            // Assert
            Assert.IsType<Values.FloatValue>(result);
            Assert.Equal((float)0, result.Value);
        }

        [Fact]
        public void ShouldOperateFloatMultiplyFloat()
        {
            // Arrange
            var float1 = new Values.FloatValue((float)1.1, A.Fake<ILogger>());
            var float2 = new Values.FloatValue((float)1.1, A.Fake<ILogger>());
            var tokenOperator = new Token(TokenGroup.OPERATORS, TokenType.MULTIPLY);
            // Act
            var result = float1.OperatedBy(tokenOperator, float2);

            // Assert
            Assert.IsType<Values.FloatValue>(result);
            Assert.Equal((float)1.21, result.Value);
        }

        [Fact]
        public void ShouldOperateFloatDivideFloat()
        {
            // Arrange
            var float1 = new Values.FloatValue((float)1.1, A.Fake<ILogger>());
            var float2 = new Values.FloatValue((float)1.1, A.Fake<ILogger>());
            var tokenOperator = new Token(TokenGroup.OPERATORS, TokenType.DIVIDE);
            // Act
            var result = float1.OperatedBy(tokenOperator, float2);

            // Assert
            Assert.IsType<Values.FloatValue>(result);
            Assert.Equal((float)1, result.Value);
        }

        [Fact]
        public void ShouldOperateFloatPowerFloat()
        {
            // Arrange
            var float1 = new Values.FloatValue((float)1.1, A.Fake<ILogger>());
            var float2 = new Values.FloatValue((float)1.1, A.Fake<ILogger>());
            var tokenOperator = new Token(TokenGroup.OPERATORS, TokenType.POWER);
            // Act
            var result = float1.OperatedBy(tokenOperator, float2);

            // Assert
            Assert.Equal((double)1.11053, Math.Round((double)result.Value, 5));
        }
    }
}