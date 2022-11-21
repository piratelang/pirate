using PirateInterpreter.StandardLibrary;

namespace PirateInterpreter.Interpreters;

public class InterpreterFactory
{
    public BaseInterpreter GetInterpreter(INode node, ILogger logger)
    {
        switch (node)
        {
            case FunctionDeclarationNode functionDeclarationNode:
                return new FunctionDeclarationInterpreter(functionDeclarationNode, this, logger);
            case FunctionCallNode functionCallNode:
                return new FunctionCallInterpreter(functionCallNode, this, logger, new StandardLibraryFactory());
            case IfStatementNode:
                return new IfStatementInterpreter(node, this, logger);
            case WhileLoopStatementNode:
                return new WhileLoopStatementInterpreter(node, this, logger);
            case ForLoopStatementNode:
                return new ForLoopStatementInterpreter(node, this, logger);
            case VariableDeclarationNode:
                return new VariableDeclarationInterpreter(node, this, logger);
            case VariableAssignmentNode:
                return new VariableAssignmentInterpreter(node, logger, this);
            case BinaryOperationNode:
                return new BinaryOperationInterpreter(node, this, logger);
            case ComparisonOperationNode:
                return new ComparisonOperationInterpreter(node, this, logger);
            case ValueNode:
                return new ValueInterpreter(node, this, logger);
        }
        throw new ArgumentNullException("node", $"Factory cannot find interpreter for {node.GetType().Name}");
    }
}
