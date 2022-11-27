using PirateInterpreter.Values.Interfaces;

namespace PirateInterpreter.Values;

/// <summary>
/// A integer value.
/// </summary>
public class IntegerValue : BaseValue, IValue
{
    public IntegerValue(object value, ILogger logger) :base(value, logger) {}

    public override BaseValue OperatedBy(Token _operator, BaseValue other)
    {
        if ((Value is not int && Value is not Int64) && (other.Value is not int && other.Value is not Int64))
        {
            throw new TypeConversionException(typeof(int));
        }
        var value = Int32.Parse(Value.ToString());
        var otherValue = Int32.Parse(Value.ToString());
        switch (_operator.TokenType)
        {
            case TokenType.PLUS:
                return new IntegerValue(value + otherValue, Logger);
            case TokenType.MINUS:
                return new IntegerValue(value - otherValue, Logger);
            case TokenType.MULTIPLY:
                return new IntegerValue(value * otherValue, Logger);
            case TokenType.DIVIDE:
                return new IntegerValue(value / otherValue, Logger);
            case TokenType.POWER:
                var doubleValue = Convert.ToDouble(Value);
                var doubleOtherValue = Convert.ToDouble(otherValue);
                return new IntegerValue(Convert.ToInt32(Math.Pow(doubleValue, doubleOtherValue)), Logger);
        }
        throw new NotImplementedException($"{_operator.TokenType.ToString()} has not been implemented");
    }
}