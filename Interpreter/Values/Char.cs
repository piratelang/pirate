using Interpreter.Values.Interfaces;
using Lexer.Enums;
using Lexer.Tokens;
using Common;
using Common.Errors;

namespace Interpreter.Values;

public class Char : BaseValue, IValue
{
    public override object Value { get; set; }
    public ILogger Logger { get; set; }

    public Char(object value, ILogger logger)
    {
        Value = value;
        Logger = logger;
    }

    public override BaseValue OperatedBy(Token _operator, BaseValue other)
    {
        if (Value is not char && Value is not string)
        {
            throw new TypeConversionException(typeof(char));
        }
        var value = (char)Value;

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
        throw new NotImplementedException($"{_operator.TokenType.ToString()} has not been implemented");
    }
}