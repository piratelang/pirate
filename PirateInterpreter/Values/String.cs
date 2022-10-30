using PirateInterpreter.Values.Interfaces;

namespace PirateInterpreter.Values;

public class String : BaseValue, IValue
{
    public String(object value, ILogger logger) :base(value, logger) {}

    public override BaseValue OperatedBy(Token _operator, BaseValue other)
    {
        if (Value is not string)
        {
            throw new TypeConversionException(typeof(string));
        }
        var value = Convert.ToString(Value);

        switch (_operator.TokenType)
        {
            case TokenOperators.PLUS:
                try
                {
                    if (other.Value is not string)
                    {
                        throw new TypeConversionException(typeof(string));
                    }
                    var otherValue = (string)other.Value;
                    return new String(string.Concat(value, otherValue), Logger);
                }
                catch (Exception ex)
                {
                    Logger.Log($" <string> + <value> returned an error of: {ex.ToString()}", this.GetType().Name, Common.Enum.LogType.ERROR);
                    throw ex;
                }

            case TokenOperators.MINUS:
                Logger.Log("<string> - <string> is not supported", this.GetType().Name, Common.Enum.LogType.ERROR);
                throw new NotImplementedException();

            case TokenOperators.MULTIPLY:
                try
                {
                    if (other.Value is not string)
                    {
                        throw new TypeConversionException(typeof(string));
                    }
                    var otherValueInt = (int)other.Value;
                    return new String(string.Concat(Enumerable.Repeat(value, otherValueInt)), Logger);
                }
                catch (Exception ex)
                {
                    Logger.Log($" <string> * <value> returned an error of: \"{ex.ToString()}\"", this.GetType().Name, Common.Enum.LogType.ERROR);
                    throw ex;
                }

            case TokenOperators.DIVIDE:
                Logger.Log("<string> / <string> is not supported", this.GetType().Name, Common.Enum.LogType.ERROR);
                throw new NotImplementedException();

            case TokenOperators.POWER:
                Logger.Log("<string> ^ <string> is not supported", this.GetType().Name, Common.Enum.LogType.ERROR);
                throw new NotImplementedException();

        }
        throw new NotImplementedException($"{_operator.TokenType.ToString()} has not been implemented");
    }
}