using PirateInterpreter.Values.Interfaces;

namespace PirateInterpreter.Values;

public class StringValue : BaseValue, IValue
{
    public StringValue(object value, ILogger logger) :base(value, logger) {}

    public override BaseValue OperatedBy(Token _operator, BaseValue other)
    {
        if (Value is not string)
        {
            throw new TypeConversionException(typeof(string));
        }
        var value = Convert.ToString(Value);

        switch (_operator.TokenType)
        {
            case TokenType.PLUS:
                try
                {
                    if (other.Value is not string)
                    {
                        throw new TypeConversionException(typeof(string));
                    }
                    var otherValue = (string)other.Value;
                    return new StringValue(string.Concat(value, otherValue), Logger);
                }
                catch (Exception ex)
                {
                    Logger.Log($" <string> + <value> returned an error of: {ex.ToString()}", Common.Enum.LogType.ERROR);
                    throw ex;
                }

            case TokenType.MINUS:
                Logger.Log("<string> - <string> is not supported", Common.Enum.LogType.ERROR);
                throw new NotImplementedException();

            case TokenType.MULTIPLY:
                try
                {
                    if (other.Value is not int)
                    {
                        throw new TypeConversionException(typeof(string));
                    }
                    var otherValueInt = (int)other.Value;
                    return new StringValue(string.Concat(Enumerable.Repeat(value, otherValueInt)), Logger);
                }
                catch (Exception ex)
                {
                    Logger.Log($" <string> * <value> returned an error of: \"{ex.ToString()}\"", Common.Enum.LogType.ERROR);
                    throw ex;
                }

            case TokenType.DIVIDE:
                Logger.Log("<string> / <string> is not supported", Common.Enum.LogType.ERROR);
                throw new NotImplementedException();

            case TokenType.POWER:
                Logger.Log("<string> ^ <string> is not supported", Common.Enum.LogType.ERROR);
                throw new NotImplementedException();

        }
        throw new NotImplementedException($"{_operator.TokenType.ToString()} has not been implemented");
    }
}