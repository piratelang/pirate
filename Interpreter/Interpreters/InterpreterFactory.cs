using Interpreter.Interpreters.Interfaces;
using Parser.Node;
using Parser.Node.Interfaces;
using Common;

namespace Interpreter.Interpreters;

public class InterpreterFactory
{
    public BaseInterpreter GetInterpreter(INode node, Logger logger)
    {
        switch (node)
        {
            case VariableAssignNode:
                return new VariableAssignNodeInterpreter(node, new InterpreterFactory(), logger);
            case BinaryOperationNode:
                return new BinaryOperationNodeInterpreter(node, new InterpreterFactory(), logger);
            case ComparisonOperationNode:
                return new ComparisonOperationNodeInterpreter(node, new InterpreterFactory(), logger);
            case ValueNode:
                return new ValueNodeInterpreter(node, logger);
        }
        return null;
    }
}
