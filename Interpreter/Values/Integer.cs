using Common.Errors;
using Interpreter.Values.Interfaces;
using Lexer.Enums;
using Lexer.Tokens;

namespace Interpreter.Values;

public class Integer : BaseValue, IValue
{
    public override object Value { get; set; }

    public Integer(object value)
    {
        Value = Convert.ToInt32(value);
    }

    public override BaseValue OperatedBy(Token _operator, BaseValue other)
    {
        if (Value is not int && other.Value is not int)
        {
            throw new TypeConversionException(typeof(int));
        }
        var value = (int)Value;
        var otherValue = (int)other.Value;
        switch (_operator.TokenType)
        {
            case TokenOperators.PLUS:
                return new Integer(value + otherValue);
            case TokenOperators.MINUS:
                return new Integer(value - otherValue);
            case TokenOperators.MULTIPLY:
                return new Integer(value * otherValue);
            case TokenOperators.DIVIDE:
                return new Integer(value / otherValue);
            case TokenOperators.POWER:
                var doubleValue = Convert.ToDouble(Value);
                var doubleOtherValue = Convert.ToDouble(otherValue);
                return new Integer(Convert.ToInt32(Math.Pow(doubleValue, doubleOtherValue)));
        }
        throw new NotImplementedException($"{_operator.TokenType.ToString()} has not been implemented");
    }
}