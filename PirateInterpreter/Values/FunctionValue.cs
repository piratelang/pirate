namespace PirateInterpreter.Values;

public class FunctionValue : BaseValue
{
    public IFunctionDeclarationNode functionDeclarationNode { get; set; }

    public FunctionValue(IFunctionDeclarationNode FunctionDeclarationNode, ILogger logger) : base(null, logger)
    {
        functionDeclarationNode = FunctionDeclarationNode;
        Logger.Log($"Created {this.GetType().Name} : \"{functionDeclarationNode.ToString()}\"", this.GetType().Name, Common.Enum.LogType.INFO);
    }

    public override string ToString()
    {
        return functionDeclarationNode.ToString();
    }

    public override BaseValue OperatedBy(Token Operator, BaseValue Value)
    {
        throw new InvalidOperationException($"Cannot operate {this.GetType().Name} by {Operator.ToString()}");
    }
}