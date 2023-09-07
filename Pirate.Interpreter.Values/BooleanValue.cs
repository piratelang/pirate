
namespace Pirate.Interpreter.Values;

/// <summary>
/// A boolean value.
/// </summary>
public class BooleanValue : BaseValue, IValue
{
    public BooleanValue(object value, ILogger logger) : base(value, logger) { }

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
                var doubleValue = Convert.ToDouble(Value);
                var doubleOtherValue = Convert.ToDouble(other.Value);
                return new IntegerValue(Convert.ToInt64(Math.Pow(doubleValue, doubleOtherValue)), Logger);
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