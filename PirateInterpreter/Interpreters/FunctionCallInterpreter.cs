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

        var functionValue = SymbolTable.Instance(Logger).GetBaseValue((string)functionCallNode.Identifier.Value.Value);
        if (functionValue is not Function) throw new TypeConversionException(functionValue.GetType(), typeof(Function));
        var function = (Function)functionValue;

        foreach (var (parameter, value) in function.functionDeclarationNode.Parameters.Zip(functionCallNode.Parameters))
        {
            SymbolTable.Instance(Logger).SetBaseValue((string)parameter.Identifier.Value.Value, InterpreterFactory.GetInterpreter(value, Logger).VisitSingleNode());
        }

        var resultList = new List<BaseValue>();
        foreach (var node in function.functionDeclarationNode.Statements)
        {
            var interpreter = InterpreterFactory.GetInterpreter(node, Logger);
            var values = interpreter.VisitNode();
            resultList.AddRange(values);
        }
        return resultList;
    }
}