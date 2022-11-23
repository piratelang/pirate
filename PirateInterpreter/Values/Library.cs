namespace PirateInterpreter.Values;

public class Library : BaseValue
{
    public string Name { get; set; }
    public List<string> Callables { get; set; }
    public Library(string name, List<string> callables, ILogger logger) : base(null, logger)
    {
        Name = name;
        Callables = callables;
        Logger.Log($"Created {this.GetType().Name}", Common.Enum.LogType.INFO);
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