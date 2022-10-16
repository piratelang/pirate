using Interpreter.Values.Interfaces;
using Lexer.Enums;
using Lexer.Tokens;

namespace Interpreter.Values;

public class Float : BaseValue, IValue
{
    public override object Value { get; set; }

    public Float(object value)
    {
        Value = value;
    }

    public override BaseValue OperatedBy(Token _operator, BaseValue other)
    {
        var value = Convert.ToDouble(Value);
        var otherValue = Convert.ToDouble(other.Value);
        switch (_operator.TokenType)
        {
            case TokenOperators.PLUS:
                return new Float(value + otherValue);
            case TokenOperators.MINUS:
                return new Float(value - otherValue);
            case TokenOperators.MULTIPLY:
                return new Float(value * otherValue);
            case TokenOperators.DIVIDE:
                return new Float(value / otherValue);
            case TokenOperators.POWER:
                var doubleValue = Convert.ToDouble(Value);
                var doubleOtherValue = Convert.ToDouble(otherValue);
                return new Float(Math.Pow(doubleValue, doubleOtherValue));
        }
        return null;
    }
}