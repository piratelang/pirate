namespace PirateInterpreter.Values;

public class FunctionValuePlaceHolder : BaseValue
{
    private string Name { get; set; }
    public FunctionValuePlaceHolder(string name, ILogger logger) : base(null, logger)
    {
        Name = name;
        Logger.Log($"Created {this.GetType().Name}", this.GetType().Name, Common.Enum.LogType.INFO);
    }

    public override string ToString()
    {
        return Name;
    }

    public override BaseValue OperatedBy(Token Operator, BaseValue Value)
    {
        throw new InvalidOperationException($"Cannot operate {this.GetType().Name} by {Operator.ToString()}");
    }
}