
using Pirate.Common.Interfaces;
using Pirate.Interpreter.Values;
using Pirate.Lexer.Enums;
using Pirate.Parser.Node.Interfaces;

namespace Pirate.Interpreter.Interpreters;

/// <summary>
/// Returns the result of the comparison operation.
/// </summary>
public class ComparisonOperationInterpreter : BaseInterpreter
{
    public IOperationNode operationNode { get; set; }
    public ComparisonOperationInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger) : base(logger, InterpreterFactory)

    {
        if (node is not IOperationNode) throw new TypeConversionException(node.GetType(), typeof(IOperationNode));
        operationNode = (IOperationNode)node;

        Logger.Log($"Created {GetType().Name} : \"{operationNode.ToString()}\"", LogType.INFO);
    }

    public override List<BaseValue> VisitNode()
    {
        Logger.Log($"Visiting {GetType().Name} : \"{operationNode.ToString()}\"", LogType.INFO);
        var interpreter = InterpreterFactory.GetInterpreter(operationNode.Left);
        var left = interpreter.VisitSingleNode();

        interpreter = InterpreterFactory.GetInterpreter(operationNode.Right);
        var right = interpreter.VisitSingleNode();

        var value = 0;

        switch (operationNode.Operator.TokenType)
        {
            case TokenType.DOUBLEEQUALS:
                value = left.Matches(right);
                break;
            case TokenType.NOTEQUALS:
                var result = left.Matches(right);
                if (result == 0) { value = 1; }
                break;
            case TokenType.GREATERTHAN:
                if ((left.Value is int || left.Value is long) && (right.Value is int || right.Value is long))
                {
                    if (Convert.ToInt64(left.Value) > Convert.ToInt64(right.Value))
                    {
                        value = 1;
                    }
                }
                break;
            case TokenType.GREATERTHANEQUALS:
                if ((left.Value is int || left.Value is long) && (right.Value is int || right.Value is long))
                {
                    if (Convert.ToInt64(left.Value) >= Convert.ToInt64(right.Value))
                    {
                        value = 1;
                    }
                }
                break;

            case TokenType.LESSTHAN:
                if ((left.Value is int || left.Value is long) && (right.Value is int || right.Value is long))
                {
                    if (Convert.ToInt64(left.Value) < Convert.ToInt64(right.Value))
                    {
                        value = 1;
                    }
                }
                break;
            case TokenType.LESSTHANEQUALS:
                if ((left.Value is int || left.Value is long) && (right.Value is int || right.Value is long))
                {
                    if (Convert.ToInt64(left.Value) >= Convert.ToInt64(right.Value))
                    {
                        value = 1;
                    }
                }
                break;
        }

        return new List<BaseValue> { new BooleanValue(value, Logger) };
    }
}