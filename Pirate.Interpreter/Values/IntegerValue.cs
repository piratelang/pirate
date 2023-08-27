using Pirate.Common.Interfaces;
using Pirate.Interpreter.Values.Interfaces;
using Pirate.Lexer.Enums;
using Pirate.Lexer.Tokens;

namespace Pirate.Interpreter.Values;

/// <summary>
/// A integer value.
/// </summary>
public class IntegerValue : BaseValue, IValue
{
    public IntegerValue(object value, ILogger logger) : base(value, logger) { }

    public override BaseValue OperatedBy(Token _operator, BaseValue other)
    {
        switch (_operator.TokenType)
        {
            case TokenType.PLUS:
                var value = ConvertValueToInt(Value);
                var otherValue = ConvertValueToInt(other.Value);

                return new IntegerValue(value + otherValue, Logger);
            case TokenType.MINUS:
                value = ConvertValueToInt(Value);
                otherValue = ConvertValueToInt(other.Value);

                return new IntegerValue(value - otherValue, Logger);
            case TokenType.MULTIPLY:
                value = ConvertValueToInt(Value);
                otherValue = ConvertValueToInt(other.Value);

                return new IntegerValue(value * otherValue, Logger);
            case TokenType.DIVIDE:
                value = ConvertValueToInt(Value);
                otherValue = ConvertValueToInt(other.Value);

                return new IntegerValue(value / otherValue, Logger);
            case TokenType.POWER:
                value = ConvertValueToInt(Value);
                otherValue = ConvertValueToInt(other.Value);

                return new IntegerValue(Convert.ToInt64(Math.Pow(value, otherValue)), Logger);
            case TokenType.MODULO:
                value = ConvertValueToInt(Value);
                otherValue = ConvertValueToInt(other.Value);

                return new IntegerValue(value % otherValue, Logger);
        }
        throw new NotImplementedException($"{_operator.TokenType.ToString()} has not been implemented");
    }

    private long ConvertValueToInt(object value)
    {
        if (value is not long)
        {
            throw new TypeConversionException(typeof(long));
        }
        return (long)value;
    }
}