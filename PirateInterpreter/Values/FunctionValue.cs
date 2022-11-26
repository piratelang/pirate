namespace PirateInterpreter.Values;

public class FunctionValue : BaseValue
{
    public IFunctionDeclarationNode FunctionDeclarationNode { get; set; }

    public FunctionValue(IFunctionDeclarationNode functionDeclarationNode, ILogger logger) : base(null, logger)
    {
        FunctionDeclarationNode = functionDeclarationNode;
        Logger.Log($"Created {this.GetType().Name} : \"{FunctionDeclarationNode.ToString()}\"", LogType.INFO);
    }

    public override string ToString()
    {
        return FunctionDeclarationNode.ToString();
    }

    public override BaseValue OperatedBy(Token Operator, BaseValue Value)
    {
        throw new InvalidOperationException($"Cannot operate {this.GetType().Name} by {Operator.ToString()}");
    }
}