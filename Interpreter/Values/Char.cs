using Interpreter.Values.Interfaces;
using Lexer.Enums;
using Lexer.Tokens;
using Common;

namespace Interpreter.Values;

public class Char : BaseValue, IValue
{
    public override object Value { get; set; }
    public Logger Logger { get; set; }

    public Char(object value, Logger logger)
    {
        Value = value;
        Logger = logger;
    }

    public override BaseValue OperatedBy(Token _operator, BaseValue other)
    {
        var value = Convert.ToChar(Value);

        switch (_operator.TokenType)
        {
            case TokenOperators.PLUS:
                Logger.Log("<char> + <char> is not supported", this.GetType().Name, Common.Enum.LogType.ERROR);
                throw new NotImplementedException();

            case TokenOperators.MINUS:
                Logger.Log("<char> - <char> is not supported", this.GetType().Name, Common.Enum.LogType.ERROR);
                throw new NotImplementedException();

            case TokenOperators.MULTIPLY:
                Logger.Log("<char> * <char> is not supported", this.GetType().Name, Common.Enum.LogType.ERROR);
                throw new NotImplementedException();

            case TokenOperators.DIVIDE:
                Logger.Log("<char> / <char> is not supported", this.GetType().Name, Common.Enum.LogType.ERROR);
                throw new NotImplementedException();

            case TokenOperators.POWER:
                Logger.Log("<char> ^ <char> is not supported", this.GetType().Name, Common.Enum.LogType.ERROR);
                throw new NotImplementedException();

        }
        return null;
    }
}