using NewInterpreterTest.Values;
using NewParserTest.Node.Interfaces;
using NewPirateLexer.Enums;

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

        var value = 0;

        switch (Node.Operator.TokenType)
        {
            case TokenComparisonOperators.DOUBLEEQUALS:
                value = left.Matches(Right);
                break;
            case TokenComparisonOperators.NOTEQUALS:
                var result = left.Matches(Right);
                if(result == 0) { value = 1; }
                break;
            case TokenComparisonOperators.GREATERHAN:
                if (left.Value is int && Right.Value is int)
                {
                    if(Convert.ToInt32(left.Value) > Convert.ToInt32(Right.Value))
                    {
                        value = 1;
                    }
                }
                break;
            case TokenComparisonOperators.GREATERTHANEQUALS:
                if (left.Value is int && Right.Value is int)
                {
                    if (Convert.ToInt32(left.Value) >= Convert.ToInt32(Right.Value))
                    {
                        value = 1;
                    }
                }
                break;

            case TokenComparisonOperators.LESSTHAN:
                if (left.Value is int && Right.Value is int)
                {
                    if (Convert.ToInt32(left.Value) < Convert.ToInt32(Right.Value))
                    {
                        value = 1;
                    }
                }
                break;
            case TokenComparisonOperators.LESSTHANEQUALS:
                if (left.Value is int && Right.Value is int)
                {
                    if (Convert.ToInt32(left.Value) >= Convert.ToInt32(Right.Value))
                    {
                        value = 1;
                    }
                }
                break;
        }

        return new Values.Boolean(value);
    }


}