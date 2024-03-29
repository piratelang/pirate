using Pirate.Interpreter.Values;

namespace Pirate.Interpreter.Interpreters;

/// <summary>
/// Returns BaseValue result of the binary operation.
/// </summary>
public class BinaryOperationInterpreter : BaseInterpreter
{
    private IOperationNode _operationNode { get; set; }

    public BinaryOperationInterpreter(INode node, InterpreterFactory interpreterFactory, ILogger logger) : base(logger, interpreterFactory)
    {
        if (node is not IOperationNode) throw new TypeConversionException(node.GetType(), typeof(IOperationNode));
        _operationNode = (IOperationNode)node;

        Logger.Log($"Created {GetType().Name} : \"{_operationNode.ToString()}\"", LogType.INFO);
    }

    public override List<BaseValue> VisitNode()
    {
        Logger.Log($"Visiting {GetType().Name} : \"{_operationNode.ToString()}\"", LogType.INFO);
        var interpreter = InterpreterFactory.GetInterpreter(_operationNode.Left);
        var left = interpreter.VisitSingleNode();

        interpreter = InterpreterFactory.GetInterpreter(_operationNode.Right);
        var Right = interpreter.VisitSingleNode();

        return new List<BaseValue> { left.OperatedBy(_operationNode.Operator, Right) };
    }
}