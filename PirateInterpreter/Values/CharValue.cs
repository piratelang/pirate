using PirateInterpreter.Values.Interfaces;

namespace PirateInterpreter.Values;

/// <summary>
/// A character value.
/// </summary>
public class CharValue : BaseValue, IValue
{
    public CharValue(object value, ILogger logger) :base(value, logger) {}

    public override BaseValue OperatedBy(Token _operator, BaseValue other)
    {
        switch (_operator.TokenType)
        {
            case TokenType.PLUS:
                var value = ConvertValueToChar(Value);
                return new StringValue(value + other.Value.ToString(), Logger);

            case TokenType.MINUS:
                Logger.Log("<char> - <char> is not supported", LogType.ERROR);
                throw new NotImplementedException();

            case TokenType.MULTIPLY:
                Logger.Log("<char> * <char> is not supported", LogType.ERROR);
                throw new NotImplementedException();

            case TokenType.DIVIDE:
                Logger.Log("<char> / <char> is not supported", LogType.ERROR);
                throw new NotImplementedException();

            case TokenType.POWER:
                Logger.Log("<char> ^ <char> is not supported", LogType.ERROR);
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