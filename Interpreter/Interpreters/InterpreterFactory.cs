using Interpreter.Interpreters.Interfaces;
using Parser.Node;
using Parser.Node.Interfaces;

namespace Interpreter.Interpreters;

public class InterpreterFactory
{
    public BaseInterpreter GetInterpreter(INode node)
    {
        switch (node)
        {
        //     case VariableAssignNode:
        //         return new VariableAssignNodeInterpreter();
            case BinaryOperationNode:
                return new BinaryOperationNodeInterpreter(node, new InterpreterFactory());
            case ComparisonOperationNode:
                return new ComparisonOperationNodeInterpreter(node, new InterpreterFactory());
            case ValueNode:
                return new ValueNodeInterpreter(node);
        }
        return null;
    }
}
