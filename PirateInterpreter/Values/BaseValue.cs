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
            Console.WriteLine("Types dont match");
            return 0;
        }
        if (!Value.Equals(other.Value))
        {
            Console.WriteLine("Values don't match");
            return 0;
        }
        return 1;
    }
}