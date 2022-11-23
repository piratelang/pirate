using PirateInterpreter.Values.Interfaces;

namespace PirateInterpreter.Values;

public class FloatValue : BaseValue, IValue
{
    public FloatValue(object value, ILogger logger) :base(value, logger) {}

    public override BaseValue OperatedBy(Token _operator, BaseValue other)
    {
        if (Value is not float && other.Value is not float)
        {
            throw new TypeConversionException(typeof(float));
        }
        var value = (float)Value;
        var otherValue = (float)other.Value;
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