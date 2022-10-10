using NewInterpreterTest.Values.Interfaces;
using NewPirateLexer.Enums;
using NewPirateLexer.Tokens;

namespace NewInterpreterTest.Values;

public class Number : IValue, INumber
{
    public int Value { get; set; }

    public Number(int value)
    {
        Value = value;
    }

    public IValue OperatedBy(Token _operator, INumber otherValue)
    {
        switch (_operator.TokenType)
        {
            case TokenOperators.PLUS:
                return new Number(Value + otherValue.Value);
            case TokenOperators.MINUS:
                return new Number(Value - otherValue.Value);
            case TokenOperators.MULTIPLY:
                return new Number(Value * otherValue.Value);
            case TokenOperators.DIVIDE:
                return new Number(Value / otherValue.Value);
            case TokenOperators.POWER:
                var doubleValue = Convert.ToDouble(Value);
                var doubleOtherValue = Convert.ToDouble(otherValue.Value);
                return new Number(Convert.ToInt32(Math.Pow(doubleValue, doubleOtherValue)));
        }
        return null;
    }
}