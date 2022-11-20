using PirateInterpreter.Values;

namespace PirateInterpreter.Interpreters;

public class FunctionCallInterpreter : BaseInterpreter
{
    public IFunctionCallNode functionCallNode { get; set; }

    public FunctionCallInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger) : base(logger, InterpreterFactory)
    {
        if (node is not IFunctionCallNode) throw new TypeConversionException(node.GetType(), typeof(IFunctionCallNode));
        functionCallNode = (IFunctionCallNode)node;

        Logger.Log($"Created {this.GetType().Name} : \"{functionCallNode.ToString()}\"", this.GetType().Name, Common.Enum.LogType.INFO);
    }

    public override List<BaseValue> VisitNode()
    {
        Logger.Log($"Visiting {this.GetType().Name} : \"{functionCallNode.ToString()}\"", this.GetType().Name, Common.Enum.LogType.INFO);

        var function = SymbolTable.Instance(Logger).GetBaseValue((string)functionCallNode.Identifier.Value.Value);
        if (function is not Function) throw new TypeConversionException(function.GetType(), typeof(Function));

        foreach (var node in functionCallNode.Parameters)
        {
            var interpreter = InterpreterFactory.GetInterpreter(node, Logger);
            var values = interpreter.VisitNode();
        }
        return new List<BaseValue>();
    }
}