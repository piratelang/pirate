using Pirate.Interpreter.Values.Interfaces;

namespace Pirate.Interpreter.Values.Function;

/// <summary>
/// A function value.
/// </summary>
public class FunctionValue : BaseValue, IFunctionValue
{
    public IFunctionDeclarationNode FunctionDeclarationNode { get; set; }

    public FunctionValue(IFunctionDeclarationNode functionDeclarationNode, ILogger logger) : base(null, logger)
    {
        FunctionDeclarationNode = functionDeclarationNode;
        Logger.Info($"Created {GetType().Name} : \"{FunctionDeclarationNode.ToString()}\"");
    }

    public override string ToString()
    {
        return FunctionDeclarationNode.ToString();
    }

    public override BaseValue OperatedBy(Token Operator, BaseValue Value)
    {
        throw new InvalidOperationException($"Cannot operate {GetType().Name} by {Operator}");
    }
}