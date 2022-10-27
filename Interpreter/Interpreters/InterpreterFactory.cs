namespace Interpreter.Interpreters;

public class InterpreterFactory
{
    public BaseInterpreter GetInterpreter(INode node, ILogger logger)
    {
        switch (node)
        {
            case VariableAssignNode:
                return new VariableAssignNodeInterpreter(node, this, logger);
            case BinaryOperationNode:
                return new BinaryOperationNodeInterpreter(node, this, logger);
            case ComparisonOperationNode:
                return new ComparisonOperationNodeInterpreter(node, this, logger);
            case ValueNode:
                return new ValueNodeInterpreter(node, this, logger);
        }
        throw new ArgumentNullException("node", $"Factory cannot find interpreter for {node.GetType().Name}");
    }
}
