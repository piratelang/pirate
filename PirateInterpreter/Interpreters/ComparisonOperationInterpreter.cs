using PirateInterpreter.Values;

namespace PirateInterpreter.Interpreters;

public class ComparisonOperationInterpreter : BaseInterpreter
{
    public IOperationNode operationNode { get; set; }
    public ComparisonOperationInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger) : base(logger, InterpreterFactory)

    {
        if (node is not IOperationNode) throw new TypeConversionException(node.GetType(), typeof(IOperationNode));
        operationNode = (IOperationNode)node;
        
        Logger.Log($"Created {this.GetType().Name} : \"{operationNode.ToString()}\"", Common.Enum.LogType.INFO);
    }

    public override List<BaseValue> VisitNode()
    {
        Logger.Log($"Visiting {this.GetType().Name} : \"{operationNode.ToString()}\"", Common.Enum.LogType.INFO);
        var interpreter = InterpreterFactory.GetInterpreter(operationNode.Left);
        var left = interpreter.VisitSingleNode();

        interpreter = InterpreterFactory.GetInterpreter(operationNode.Right);
        var right = interpreter.VisitSingleNode();

        var value = 0;

        switch (operationNode.Operator.TokenType)
        {
            case TokenType.DOUBLEEQUALS:
                left.Value = Convert.ToInt32(left.Value);
                right.Value = Convert.ToInt32(right.Value);
                value = left.Matches(right);
                break;
            case TokenType.NOTEQUALS:
                var result = left.Matches(right);
                if(result == 0) { value = 1; }
                break;
            case TokenType.GREATERTHAN:
                if ((left.Value is int || left.Value is Int64) && (right.Value is int || right.Value is Int64))
                {
                    if(Convert.ToInt32(left.Value) > Convert.ToInt32(right.Value))
                    {
                        value = 1;
                    }
                }
                break;
            case TokenType.GREATERTHANEQUALS:
                if ((left.Value is int || left.Value is Int64) && (right.Value is int || right.Value is Int64))
                {
                    if (Convert.ToInt32(left.Value) >= Convert.ToInt32(right.Value))
                    {
                        value = 1;
                    }
                }
                break;

            case TokenType.LESSTHAN:
                if ((left.Value is int || left.Value is Int64) && (right.Value is int || right.Value is Int64))
                {
                    if (Convert.ToInt32(left.Value) < Convert.ToInt32(right.Value))
                    {
                        value = 1;
                    }
                }
                break;
            case TokenType.LESSTHANEQUALS:
                if ((left.Value is int || left.Value is Int64) && (right.Value is int || right.Value is Int64))
                {
                    if (Convert.ToInt32(left.Value) >= Convert.ToInt32(right.Value))
                    {
                        value = 1;
                    }
                }
                break;
        }

        return new List<BaseValue> { new BooleanValue(value, Logger) };
    }
}