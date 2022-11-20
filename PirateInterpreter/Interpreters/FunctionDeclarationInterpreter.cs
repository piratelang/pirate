using PirateInterpreter.Values;

namespace PirateInterpreter.Interpreters;

public class FunctionDeclarationInterpreter : BaseInterpreter
{
    public IFunctionDeclarationNode functionDeclarationNode { get; set; }

    public FunctionDeclarationInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger) : base(logger, InterpreterFactory)
    {
        if (node is not IFunctionDeclarationNode) throw new TypeConversionException(node.GetType(), typeof(IFunctionDeclarationNode));
        functionDeclarationNode = (IFunctionDeclarationNode)node;

        Logger.Log($"Created {this.GetType().Name} : \"{functionDeclarationNode.ToString()}\"", this.GetType().Name, Common.Enum.LogType.INFO);
    }

    public override List<BaseValue> VisitNode()
    {
        Logger.Log($"Visiting {this.GetType().Name} : \"{functionDeclarationNode.ToString()}\"", this.GetType().Name, Common.Enum.LogType.INFO);

        var function = new Function(functionDeclarationNode, Logger);
        SymbolTable.Instance(Logger).SetBaseValue((string)functionDeclarationNode.Identifier.Value.Value, function);
        return new List<BaseValue>();
    }
}