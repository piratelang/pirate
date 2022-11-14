using PirateInterpreter.Values;

namespace PirateInterpreter.Interpreters;

public class ComparisonOperationNodeInterpreter : BaseInterpreter
{
    public IOperationNode operationNode { get; set; }
    public ComparisonOperationNodeInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger) : base(logger, InterpreterFactory)
    {
        if (node is not IOperationNode) throw new TypeConversionException(node.GetType(), typeof(IOperationNode));
        operationNode = (IOperationNode)node;
        
        Logger.Log($"Created {this.GetType().Name} : \"{operationNode.ToString()}\"", this.GetType().Name, Common.Enum.LogType.INFO);
    }

    public override List<BaseValue> VisitNode()
    {
        var interpreter = InterpreterFactory.GetInterpreter(operationNode.Left, Logger );
        var left = interpreter.VisitSingleNode();

        interpreter = InterpreterFactory.GetInterpreter(operationNode.Right, Logger);
        var right = interpreter.VisitSingleNode();

        var value = 0;

        switch (operationNode.Operator.TokenType)
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