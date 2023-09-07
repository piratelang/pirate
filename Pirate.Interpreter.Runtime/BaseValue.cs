using Pirate.Common.Logger.Interfaces;
using Pirate.Interpreter.Runtime.Interfaces;
using Pirate.Lexer.Tokens;

namespace Pirate.Interpreter.Runtime;

/// <summary>
/// Represents a value resulting from a interpreted node.
/// </summary>
public abstract class BaseValue : IValue
{
    public object? Value { get; set; }
    public ILogger Logger { get; set; }

    public BaseValue(object? value, ILogger logger)
    {
        Logger = logger;
        Value = value;
    }

    public abstract BaseValue OperatedBy(Token _operator, BaseValue other);

    public int Matches(BaseValue other)
    {
        if (Value.GetType() != other.Value.GetType())
        {
            Logger.Info($"Types dont match. {Value.GetType()} : {Value} | {other.Value.GetType()} : {other.Value}");
            return 0;
        }
        if (!Value.Equals(other.Value))
        {
            Logger.Info($"Values don't match. {Value} | {other.Value}");
            return 0;
        }
        return 1;
    }
}