using Interpreter.Values;
using Parser.Node.Interfaces;

namespace Interpreter.Interpreters;

public class BinaryOperationNodeInterpreter : BaseInterpreter
{
    public IOperationNode Node { get; set; }
    private InterpreterFactory interpreterFactory;
    public BinaryOperationNodeInterpreter(INode node, InterpreterFactory InterpreterFactory)
    {
        Node = node as IOperationNode;
        interpreterFactory = InterpreterFactory;
    }

    public override BaseValue VisitNode()
    {
        var interpreter = interpreterFactory.GetInterpreter(Node.Left);
        var left = interpreter.VisitNode();

        interpreter = interpreterFactory.GetInterpreter(Node.Right);
        var Right = interpreter.VisitNode();
        
        return left.OperatedBy(Node.Operator, Right);
    }
}