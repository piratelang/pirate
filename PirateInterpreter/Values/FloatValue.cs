using PirateInterpreter.Values.Interfaces;

namespace PirateInterpreter.Values;

/// <summary>
/// A float value.
/// </summary>
public class FloatValue : BaseValue, IValue
{
    public FloatValue(object value, ILogger logger) :base(value, logger) {}

    public override BaseValue OperatedBy(Token _operator, BaseValue other)
    {
        if (Value is not double && other.Value is not double)
        {
            throw new TypeConversionException(typeof(double));
        }
        var value = (double)Value;
        var otherValue = (double)other.Value;
        switch (_operator.TokenType)
        {
            case TokenType.PLUS:
                return new FloatValue(value + otherValue, Logger);
            case TokenType.MINUS:
                return new FloatValue(value - otherValue, Logger);
            case TokenType.MULTIPLY:
                return new FloatValue(value * otherValue, Logger);
            case TokenType.DIVIDE:
                return new FloatValue(value / otherValue, Logger);
            case TokenType.POWER:
                var doubleValue = Convert.ToDouble(Value);
                var doubleOtherValue = Convert.ToDouble(otherValue);
                return new FloatValue(Math.Pow(doubleValue, doubleOtherValue), Logger);
        }
        throw new NotImplementedException($"{_operator.TokenType.ToString()} has not been implemented");
    }
}