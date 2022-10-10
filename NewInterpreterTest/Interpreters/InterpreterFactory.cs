using NewInterpreterTest.Interpreters.Interfaces;
using NewParserTest.Node;
using NewParserTest.Node.Interfaces;

namespace NewInterpreterTest.Interpreters;

public class InterpreterFactory
{
    public IInterpreter GetInterpreter(INode node)
    {
        switch (node)
        {
            case VariableAssignNode:
                return new VariableAssignNodeInterpreter();
            case BinaryOperationNode:
                return new BinaryOperationNodeInterpreter();
            case ComparisonOperationNode:
                return new ComparisonOperationNodeInterpreter();
            case ValueNode:
                return new ValueNodeInterpreter();
        }
        return null;
    }
}
