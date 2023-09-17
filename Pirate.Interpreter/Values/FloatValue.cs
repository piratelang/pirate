
using Pirate.Interpreter.Values.Interfaces;

namespace Pirate.Interpreter.Values;

/// <summary>
/// A float value.
/// </summary>
public class FloatValue : BaseValue, IValue
{
    public FloatValue(object value, ILogger logger) : base(value, logger) { }

    public override BaseValue OperatedBy(Token _operator, BaseValue other)
    {
        switch (_operator.TokenType)
        {
            case TokenType.PLUS:
                var value = ConvertValueToDoubleOrInt(Value);
                var otherValue = ConvertValueToDoubleOrInt(other.Value);

                return new FloatValue(value + otherValue, Logger);
            case TokenType.MINUS:
                value = ConvertValueToDoubleOrInt(Value);
                otherValue = ConvertValueToDoubleOrInt(other.Value);

                return new FloatValue(value - otherValue, Logger);
            case TokenType.MULTIPLY:
                value = ConvertValueToDoubleOrInt(Value);
                otherValue = ConvertValueToDoubleOrInt(other.Value);

                return new FloatValue(value * otherValue, Logger);
            case TokenType.DIVIDE:
                value = ConvertValueToDoubleOrInt(Value);
                otherValue = ConvertValueToDoubleOrInt(other.Value);

                return new FloatValue(value / otherValue, Logger);
            case TokenType.POWER:
                value = ConvertValueToDoubleOrInt(Value);
                otherValue = ConvertValueToDoubleOrInt(other.Value);
                return new FloatValue(Math.Pow(value, otherValue), Logger);
            case TokenType.MODULO:
                value = ConvertValueToDoubleOrInt(Value);
                otherValue = ConvertValueToDoubleOrInt(other.Value);
                return new FloatValue(value % otherValue, Logger);
        }
        throw new NotImplementedException($"{_operator.TokenType.ToString()} has not been implemented");
    }
    private double ConvertValueToDoubleOrInt(object value)
    {
        if (value is not double && value is not long)
        {
            throw new TypeConversionException(typeof(double));
        }
        return value is double ? (double)value : (long)value;
    }
}