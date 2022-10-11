using NewInterpreterTest.Values;
using NewParserTest.Node.Interfaces;

namespace NewInterpreterTest.Interpreters;

public class ComparisonOperationNodeInterpreter : BaseInterpreter
{
    public IOperationNode Node { get; set; }
    private InterpreterFactory interpreterFactory;
    public ComparisonOperationNodeInterpreter(INode node, InterpreterFactory InterpreterFactory)
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

        var matches = left.Matches(Right);
    }
}