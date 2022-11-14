using PirateInterpreter.Values;

namespace PirateInterpreter.Interpreters;

public class BinaryOperationInterpreter : BaseInterpreter
{
    private IOperationNode _node { get; set; }

    public BinaryOperationInterpreter(INode node, InterpreterFactory interpreterFactory, ILogger logger) : base(logger, interpreterFactory)
    {
        if (node is not IOperationNode) throw new TypeConversionException(node.GetType(), typeof(IOperationNode));            
        _node = (IOperationNode)node;
        
        Logger.Log($"Created {this.GetType().Name} : \"{_node.ToString()}\"", this.GetType().Name, Common.Enum.LogType.INFO);
    }

    public override List<BaseValue> VisitNode()
    {
        var interpreter = InterpreterFactory.GetInterpreter(_node.Left, Logger);
        var left = interpreter.VisitNode();

        if (left.Count > 1) throw new Exception("Left value is not a single value");

        interpreter = InterpreterFactory.GetInterpreter(_node.Right, Logger);
        var Right = interpreter.VisitNode();

        if (Right.Count > 1) throw new Exception("Right value is not a single value");
        
        return new List<BaseValue> {left[0].OperatedBy(_node.Operator, Right[0])};
    }
}