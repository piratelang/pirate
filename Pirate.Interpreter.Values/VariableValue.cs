namespace Pirate.Interpreter.Values;

/// <summary>
/// A variable value.
/// </summary>
public class VariableValue : BaseValue, IValue
{
    private IRuntime Runtime { get; set; }

    public VariableValue(object value, ILogger logger, IRuntime runtime) : base(value, logger)
    {   

        Value = runtime.Variables.Get((string)value).Value;
        Runtime = runtime;
    }

    public override BaseValue OperatedBy(Token _operator, BaseValue other)
    {
        Logger.Info($"Variable {Value.ToString()}, {Value.GetType()} is being operated by {other.ToString()}, {other.GetType()} with {_operator.ToString()}");
        switch (Value.GetType())
        {
            case Type when Value.GetType() == typeof(int):
            case Type when Value.GetType() == typeof(long):
            case Type when Value.GetType() == typeof(int):
                return new IntegerValue(Value, Logger).OperatedBy(_operator, other);
            case Type stringType when Value.GetType() == typeof(string):
                return new StringValue(Value, Logger).OperatedBy(_operator, other);
            case Type floatType when Value.GetType() == typeof(float):
                return new FloatValue(Value, Logger).OperatedBy(_operator, other);
            case Type charType when Value.GetType() == typeof(char):
                return new CharValue(Value, Logger).OperatedBy(_operator, other);
            case Type when Value.GetType() == typeof(VariableValue):
                return new VariableValue(Value, Logger, ((VariableValue)Value).Runtime).OperatedBy(_operator, other);
        }
        throw new NotImplementedException("No TypeCode found");
    }
}