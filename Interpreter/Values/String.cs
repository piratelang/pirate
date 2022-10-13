using Interpreter.Values.Interfaces;
using Lexer.Enums;
using Lexer.Tokens;
using Common;

namespace Interpreter.Values;

public class String : BaseValue, IValue
{
    public override object Value { get; set; }
    public Logger Logger { get; set; }

    public String(object value, Logger logger)
    {
        Value = value;
        Logger = logger;
    }

    public override BaseValue OperatedBy(Token _operator, BaseValue other)
    {
        var value = Convert.ToString(Value);

        switch (_operator.TokenType)
        {
            case TokenOperators.PLUS:
                try
                {
                    var otherValue = Convert.ToString(other.Value);
                    return new String(string.Concat(value, otherValue), Logger);
                }
                catch (Exception ex)
                {
                    Logger.Log($" <string> + <value> returned an error of: {ex.ToString()}", this.GetType().Name, Common.Enum.LogType.ERROR);
                    throw;
                }

            case TokenOperators.MINUS:
                Logger.Log("<string> - <string> is not supported", this.GetType().Name, Common.Enum.LogType.ERROR);
                throw new NotImplementedException();

            case TokenOperators.MULTIPLY:
                try
                {
                    var otherValueInt = Convert.ToInt32(other.Value);
                    return new String(string.Concat(Enumerable.Repeat(value, otherValueInt)), Logger);
                }
                catch (Exception ex)
                {
                    Logger.Log($" <string> * <value> returned an error of: \"{ex.ToString()}\"", this.GetType().Name, Common.Enum.LogType.ERROR);
                    throw;
                }

            case TokenOperators.DIVIDE:
                Logger.Log("<string> / <string> is not supported", this.GetType().Name, Common.Enum.LogType.ERROR);
                throw new NotImplementedException();

            case TokenOperators.POWER:
                Logger.Log("<string> ^ <string> is not supported", this.GetType().Name, Common.Enum.LogType.ERROR);
                throw new NotImplementedException();

        }
        return null;
    }
}