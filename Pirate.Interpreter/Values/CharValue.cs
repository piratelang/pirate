
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
                Logger.Error("<char> - <char> is not supported");
                throw new NotImplementedException();

            case TokenType.MULTIPLY:
                Logger.Error("<char> * <char> is not supported");
                throw new NotImplementedException();

            case TokenType.DIVIDE:
                Logger.Error("<char> / <char> is not supported");
                throw new NotImplementedException();

            case TokenType.POWER:
                Logger.Error("<char> ^ <char> is not supported");
                throw new NotImplementedException();

            case TokenType.MODULO:
                Logger.Error("<char> % <char> is not supported");
                throw new NotImplementedException();

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