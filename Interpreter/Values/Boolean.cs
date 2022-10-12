using Interpreter.Values.Interfaces;
using Lexer.Enums;
using Lexer.Tokens;

namespace Interpreter.Values;

public class Boolean : BaseValue, IValue
{
    public override object Value { get; set; }

    public Boolean(object value)
    {
        Value = Convert.ToInt32(value);
    }

    public override BaseValue OperatedBy(Token _operator, BaseValue other)
    {
        var value = Convert.ToInt32(Value);
        var otherValue = Convert.ToInt32(other.Value);
        switch (_operator.TokenType)
        {
            case TokenOperators.PLUS:
                return new Number(value + otherValue);
            case TokenOperators.MINUS:
                return new Number(value - otherValue);
            case TokenOperators.MULTIPLY:
                return new Number(value * otherValue);
            case TokenOperators.DIVIDE:
                return new Number(value / otherValue);
            case TokenOperators.POWER:
                var doubleValue = Convert.ToDouble(Value);
                var doubleOtherValue = Convert.ToDouble(otherValue);
                return new Number(Convert.ToInt32(Math.Pow(doubleValue, doubleOtherValue)));
        }
        return null;
    }
}