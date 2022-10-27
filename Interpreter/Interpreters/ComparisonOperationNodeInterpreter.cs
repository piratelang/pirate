using Interpreter.Values;

namespace Interpreter.Interpreters;

public class ComparisonOperationNodeInterpreter : BaseInterpreter
{
    public IOperationNode Node { get; set; }
    public ComparisonOperationNodeInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger) : base(logger, InterpreterFactory)
    {
        if (node is not IOperationNode) throw new TypeConversionException(node.GetType(), typeof(IOperationNode));
        Node = (IOperationNode)node;
        
        Logger.Log($"Created {this.GetType().Name} : \"{Node.ToString()}\"", this.GetType().Name, Common.Enum.LogType.INFO);
    }

    public override BaseValue VisitNode()
    {
        var interpreter = InterpreterFactory.GetInterpreter(Node.Left, Logger );
        var left = interpreter.VisitNode();

        interpreter = InterpreterFactory.GetInterpreter(Node.Right, Logger);
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

        return new Values.Boolean(value, Logger);
    }


}