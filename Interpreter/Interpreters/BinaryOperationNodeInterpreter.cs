using Interpreter.Values;
using Parser.Node.Interfaces;
using Common;
using Common.Errors;

namespace Interpreter.Interpreters;

public class BinaryOperationNodeInterpreter : BaseInterpreter
{
    public IOperationNode Node { get; set; }
    private InterpreterFactory interpreterFactory;
    public ILogger Logger { get; set; }
    public BinaryOperationNodeInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger)
    {
        if (node is not IOperationNode)
        {
            throw new TypeConversionException(node.GetType(), typeof(IOperationNode));            
        }
        Node = (IOperationNode)node;

        interpreterFactory = InterpreterFactory;
        Logger = logger;
        Logger.Log($"Created {this.GetType().Name} : \"{Node.ToString()}\"", this.GetType().Name, Common.Enum.LogType.INFO);
    }

    public override BaseValue VisitNode()
    {
        var interpreter = interpreterFactory.GetInterpreter(Node.Left, Logger);
        var left = interpreter.VisitNode();

        interpreter = interpreterFactory.GetInterpreter(Node.Right, Logger);
        var Right = interpreter.VisitNode();
        
        return left.OperatedBy(Node.Operator, Right);
    }
}