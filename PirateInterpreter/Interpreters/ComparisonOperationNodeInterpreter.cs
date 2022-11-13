using PirateInterpreter.Values;

namespace PirateInterpreter.Interpreters;

public class ComparisonOperationNodeInterpreter : BaseInterpreter
{
    public IOperationNode Node { get; set; }
    public ComparisonOperationNodeInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger) : base(logger, InterpreterFactory)
    {
        if (node is not IOperationNode) throw new TypeConversionException(node.GetType(), typeof(IOperationNode));
        Node = (IOperationNode)node;
        
        Logger.Log($"Created {this.GetType().Name} : \"{Node.ToString()}\"", this.GetType().Name, Common.Enum.LogType.INFO);
    }

    public override List<BaseValue> VisitNode()
    {
        var interpreter = InterpreterFactory.GetInterpreter(Node.Left, Logger );
        var leftNode = interpreter.VisitNode();

        if (leftNode.Count > 1) throw new Exception("Left value is not a single value");
        var left = leftNode[0];

        interpreter = InterpreterFactory.GetInterpreter(Node.Right, Logger);
        var rightNode = interpreter.VisitNode();  

        if (rightNode.Count > 1) throw new Exception("Right value is not a single value");
        var right = rightNode[0];

        var value = 0;

        switch (Node.Operator.TokenType)
        {
            case TokenComparisonOperators.DOUBLEEQUALS:
                value = left.Matches(right);
                break;
            case TokenComparisonOperators.NOTEQUALS:
                var result = left.Matches(right);
                if(result == 0) { value = 1; }
                break;
            case TokenComparisonOperators.GREATERTHAN:
                if (left.Value is int && right.Value is int)
                {
                    if(Convert.ToInt32(left.Value) > Convert.ToInt32(right.Value))
                    {
                        value = 1;
                    }
                }
                break;
            case TokenComparisonOperators.GREATERTHANEQUALS:
                if (left.Value is int && right.Value is int)
                {
                    if (Convert.ToInt32(left.Value) >= Convert.ToInt32(right.Value))
                    {
                        value = 1;
                    }
                }
                break;

            case TokenComparisonOperators.LESSTHAN:
                if (left.Value is int && right.Value is int)
                {
                    if (Convert.ToInt32(left.Value) < Convert.ToInt32(right.Value))
                    {
                        value = 1;
                    }
                }
                break;
            case TokenComparisonOperators.LESSTHANEQUALS:
                if (left.Value is int && right.Value is int)
                {
                    if (Convert.ToInt32(left.Value) >= Convert.ToInt32(right.Value))
                    {
                        value = 1;
                    }
                }
                break;
        }

        return new List<BaseValue> { new Values.Boolean(value, Logger) };
    }
}