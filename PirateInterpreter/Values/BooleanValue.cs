using PirateInterpreter.Values.Interfaces;

namespace PirateInterpreter.Values;

/// <summary>
/// A boolean value.
/// </summary>
public class BooleanValue : BaseValue, IValue
{
    public BooleanValue(object value, ILogger logger) :base(value, logger) {}

    public override BaseValue OperatedBy(Token _operator, BaseValue other)
    {
        switch (_operator.TokenType)
        {
            case TokenType.PLUS:
                return new IntegerValue(ConvertValueToInt(Value) + ConvertValueToInt(other.Value), Logger);
            case TokenType.MINUS:
                return new IntegerValue(ConvertValueToInt(Value) - ConvertValueToInt(other.Value), Logger);
            case TokenType.MULTIPLY:
                return new IntegerValue(ConvertValueToInt(Value) * ConvertValueToInt(other.Value), Logger);
            case TokenType.DIVIDE:
                return new IntegerValue(ConvertValueToInt(Value) / ConvertValueToInt(other.Value), Logger);
            case TokenType.POWER:
                var doubleValue = Convert.ToDouble(Value);
                var doubleOtherValue = Convert.ToDouble(other.Value);
                return new IntegerValue(Convert.ToInt64(Math.Pow(doubleValue, doubleOtherValue)), Logger);
        }
        throw new NotImplementedException($"{_operator.TokenType.ToString()} has not been implemented");
    }

    private Int64 ConvertValueToInt(object value)
    {
        if (value is not Int64)
        {
            throw new TypeConversionException(typeof(Int64));
        }
        return (Int64)value;
    }

}