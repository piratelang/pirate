using PirateInterpreter.Values.Interfaces;

namespace PirateInterpreter.Values;

public class Boolean : BaseValue, IValue
{
    public Boolean(object value, ILogger logger) :base(value, logger) {}

    public override BaseValue OperatedBy(Token _operator, BaseValue other)
    {
        if (Value is not int || other.Value is not int)
        {
            throw new TypeConversionException(typeof(int));
        }
        var value = (int)Value;
        var otherValue = (int)other.Value;
        switch (_operator.TokenType)
        {
            case TokenOperators.PLUS:
                return new Integer(value + otherValue, Logger);
            case TokenOperators.MINUS:
                return new Integer(value - otherValue, Logger);
            case TokenOperators.MULTIPLY:
                return new Integer(value * otherValue, Logger);
            case TokenOperators.DIVIDE:
                return new Integer(value / otherValue, Logger);
            case TokenOperators.POWER:
                var doubleValue = Convert.ToDouble(Value);
                var doubleOtherValue = Convert.ToDouble(otherValue);
                return new Integer(Convert.ToInt32(Math.Pow(doubleValue, doubleOtherValue)), Logger);
        }
        throw new NotImplementedException($"{_operator.TokenType.ToString()} has not been implemented");
    }
}