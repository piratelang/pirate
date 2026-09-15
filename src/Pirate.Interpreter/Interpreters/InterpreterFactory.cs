//using Pirate.Interpreter.StandardLibrary.Interfaces;
using Pirate.Interpreter.Interpreters.Interfaces;
using Pirate.Interpreter.Interfaces;

namespace Pirate.Interpreter.Interpreters;

public class InterpreterFactory : IInterpreterFactory
{
    private IStandardLibraryProvider StandardLibraryFactory;
    private ILogger Logger;
    private IRuntime Runtime;

    public InterpreterFactory(IStandardLibraryProvider standardLibraryFactory, ILogger logger, IRuntime runtime)
    {
        StandardLibraryFactory = standardLibraryFactory;
        Logger = logger;
        Runtime = runtime;
    }

    public BaseInterpreter GetInterpreter(INode node)
    {
        switch (node)
        {
            case FunctionDeclarationNode:
                return new FunctionDeclarationInterpreter(node, this, Logger, Runtime);
            case FunctionCallNode:
                return new FunctionCallInterpreter(node, this, Logger, StandardLibraryFactory, Runtime);
            case IfStatementNode:
                return new IfStatementInterpreter(node, this, Logger);
            case WhileLoopStatementNode:
                return new WhileLoopStatementInterpreter(node, this, Logger);
            case ForLoopStatementNode:
                return new ForLoopStatementInterpreter(node, this, Logger, Runtime);
            case VariableDeclarationNode:
                return new VariableDeclarationInterpreter(node, this, Logger, Runtime);
            case VariableAssignmentNode:
                return new VariableAssignmentInterpreter(node, Logger, this, Runtime);
            case BinaryOperationNode:
                return new BinaryOperationInterpreter(node, this, Logger);
            case ComparisonOperationNode:
                return new ComparisonOperationInterpreter(node, this, Logger);
            case ValueNode:
                return new ValueInterpreter(node, this, Logger, Runtime);
            case CommentNode:
                return new CommentInterpreter(node, this, Logger);
            case ExternNode:
                return new ExternInterpreter(node, Runtime, StandardLibraryFactory, Logger, this);
        }
        throw new ArgumentNullException("node", $"Factory cannot find interpreter for {node.GetType().Name}");
    }
}
