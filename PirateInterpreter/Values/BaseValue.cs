using PirateInterpreter.Values.Interfaces;

namespace PirateInterpreter.Values;

public abstract class BaseValue : IValue
{
    public object Value {get; set;}
    public ILogger Logger { get; set; }
    
    public BaseValue(object value, ILogger logger)
    {
        Logger = logger;
        Value = value;
    }


    public abstract BaseValue OperatedBy(Token _operator, BaseValue other);

    public int Matches(BaseValue other)
    {
        if (Value.GetType() != other.Value.GetType())
        {
            Logger.Log($"Types dont match. {Value} | {other.Value}", Common.Enum.LogType.INFO);
            return 0;
        }
        if (!Value.Equals(other.Value))
        {
            Logger.Log($"Values don't match. {Value} | {other.Value}", Common.Enum.LogType.INFO);
            return 0;
        }
        return 1;
    }
}