
using Pirate.Interpreter.Values.Interfaces;

namespace Pirate.Interpreter.Values;

/// <summary>
/// A character value.
/// </summary>
public class CharValue : BaseValue, IValue
{
    public CharValue(object value, ILogger logger) : base(value, logger) { }

    public override BaseValue OperatedBy(Token _operator, BaseValue other)
    {
        switch (_operator.TokenType)
        {
            case TokenType.PLUS:
                var value = ConvertValueToChar(Value);
                return new StringValue(value + other.Value.ToString(), Logger);

            case TokenType.MINUS:
                Logger.Error(new NotImplementedException("<char> - <char> is not supported"));
                throw new NotImplementedException("<char> - <char> is not supported");

            case TokenType.MULTIPLY:
                Logger.Error(new NotImplementedException("<char> * <char> is not supported"));
                throw new NotImplementedException("<char> * <char> is not supported");

            case TokenType.DIVIDE:
                Logger.Error(new NotImplementedException("<char> / <char> is not supported"));
                throw new NotImplementedException("<char> / <char> is not supported");

            case TokenType.POWER:
                Logger.Error(new NotImplementedException("<char> ^ <char> is not supported"));
                throw new NotImplementedException("<char> ^ <char> is not supported");

            case TokenType.MODULO:
                Logger.Error(new NotImplementedException("<char> % <char> is not supported"));
                throw new NotImplementedException("<char> % <char> is not supported");

        }
        throw new NotImplementedException($"{_operator.TokenType.ToString()} has not been implemented");
    }

    private char ConvertValueToChar(object value)
    {
        if (value is not char)
        {
            throw new TypeConversionException(typeof(char));
        }
        return (char)value;
    }
}