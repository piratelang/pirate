
using Pirate.Interpreter.Values.Interfaces;

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
                Logger.Error(new NotImplementedException("<string> - <string> is not supported"));
                throw new NotImplementedException("<string> - <string> is not supported");
            case TokenType.MULTIPLY:
                value = ConvertValueToString(Value);
                return new StringValue(string.Concat(Enumerable.Repeat(value, ConvertValueToInt(other.Value))), Logger);
            case TokenType.DIVIDE:
                Logger.Error(new NotImplementedException("<string> / <string> is not supported"));
                throw new NotImplementedException("<string> / <string> is not supported");
            case TokenType.POWER:
                Logger.Error(new NotImplementedException("<string> ^ <string> is not supported"));
                throw new NotImplementedException("<string> ^ <string> is not supported");
            case TokenType.MODULO:
                Logger.Error(new NotImplementedException("<string> % <string> is not supported"));
                throw new NotImplementedException("<string> % <string> is not supported");

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