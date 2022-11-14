using PirateInterpreter.Values;

namespace PirateInterpreter.Interpreters;

public class BinaryOperationInterpreter : BaseInterpreter
{
    private IOperationNode _operationNode { get; set; }

    public BinaryOperationInterpreter(INode node, InterpreterFactory interpreterFactory, ILogger logger) : base(logger, interpreterFactory)
    {
        if (node is not IOperationNode) throw new TypeConversionException(node.GetType(), typeof(IOperationNode));            
        _operationNode = (IOperationNode)node;
        
        Logger.Log($"Created {this.GetType().Name} : \"{_operationNode.ToString()}\"", this.GetType().Name, Common.Enum.LogType.INFO);
    }

    public override List<BaseValue> VisitNode()
    {
        var interpreter = InterpreterFactory.GetInterpreter(_operationNode.Left, Logger);
        var left = interpreter.VisitSingleNode();

        interpreter = InterpreterFactory.GetInterpreter(_operationNode.Right, Logger);
        var Right = interpreter.VisitSingleNode();
        
        return new List<BaseValue> {left.OperatedBy(_operationNode.Operator, Right)};
    }
}