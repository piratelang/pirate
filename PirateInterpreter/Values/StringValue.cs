using Pirate.Common.Enum;
using Pirate.Common.Interfaces;
using Pirate.Interpreter.Values.Interfaces;
using Pirate.Lexer.Enums;
using Pirate.Lexer.Tokens;

namespace Pirate.Interpreter.Values;

/// <summary>
/// A string value.
/// </summary>
public class StringValue : BaseValue, IValue
{
    public StringValue(object value, ILogger logger) : base(value, logger) { }

    public override BaseValue OperatedBy(Token _operator, BaseValue other)
    {
        switch (_operator.TokenType)
        {
            case TokenType.PLUS:
                var value = ConvertValueToString(Value);
                var otherValue = ConvertValueToString(other.Value);
                return new StringValue(value + otherValue, Logger);
            case TokenType.MINUS:
                Logger.Log("<string> - <string> is not supported", LogType.ERROR);
                throw new NotImplementedException();
            case TokenType.MULTIPLY:
                value = ConvertValueToString(Value);
                return new StringValue(string.Concat(Enumerable.Repeat(value, ConvertValueToInt(other.Value))), Logger);
            case TokenType.DIVIDE:
                Logger.Log("<string> / <string> is not supported", LogType.ERROR);
                throw new NotImplementedException();
            case TokenType.POWER:
                Logger.Log("<string> ^ <string> is not supported", LogType.ERROR);
                throw new NotImplementedException();
            case TokenType.MODULO:
                Logger.Log("<string> % <string> is not supported", LogType.ERROR);
                throw new NotImplementedException();

        }
        throw new NotImplementedException($"{_operator.TokenType.ToString()} has not been implemented");
    }

    private string ConvertValueToString(object value)
    {
        if (value is not string)
        {
            throw new TypeConversionException(typeof(string));
        }
        return (string)value;
    }

    private int ConvertValueToInt(object value)
    {
        if (value is not int)
        {
            throw new TypeConversionException(typeof(int));
        }
        return (int)value;
    }
}