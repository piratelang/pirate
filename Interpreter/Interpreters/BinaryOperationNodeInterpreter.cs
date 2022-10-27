using Interpreter.Values;

namespace Interpreter.Interpreters;

public class BinaryOperationNodeInterpreter : BaseInterpreter
{
    private IOperationNode _node { get; set; }

    public BinaryOperationNodeInterpreter(INode node, InterpreterFactory interpreterFactory, ILogger logger) : base(logger, interpreterFactory)
    {
        if (node is not IOperationNode) throw new TypeConversionException(node.GetType(), typeof(IOperationNode));            
        _node = (IOperationNode)node;
        
        Logger.Log($"Created {this.GetType().Name} : \"{_node.ToString()}\"", this.GetType().Name, Common.Enum.LogType.INFO);
    }

    public override BaseValue VisitNode()
    {
        var interpreter = InterpreterFactory.GetInterpreter(_node.Left, Logger);
        var left = interpreter.VisitNode();

        interpreter = InterpreterFactory.GetInterpreter(_node.Right, Logger);
        var Right = interpreter.VisitNode();
        
        return left.OperatedBy(_node.Operator, Right);
    }
}