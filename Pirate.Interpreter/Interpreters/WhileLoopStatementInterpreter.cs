
using Pirate.Common.Interfaces;
using Pirate.Interpreter.Values;
using Pirate.Parser.Node.Interfaces;

namespace Pirate.Interpreter.Interpreters;

/// <summary>
/// Executes the while loop statements while the condition is true.
/// </summary>
public class WhileLoopStatementInterpreter : BaseInterpreter
{
    public IWhileLoopStatementNode whileLoopStatementNode { get; set; }

    public WhileLoopStatementInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger) : base(logger, InterpreterFactory)
    {
        if (node is not IWhileLoopStatementNode) throw new TypeConversionException(node.GetType(), typeof(IIfStatementNode));
        whileLoopStatementNode = (IWhileLoopStatementNode)node;

        Logger.Log($"Created {GetType().Name} : \"{whileLoopStatementNode.ToString()}\"", LogType.INFO);
    }
    public override List<BaseValue> VisitNode()
    {
        Logger.Log($"Visiting {GetType().Name} : \"{whileLoopStatementNode.ToString()}\"", LogType.INFO);
        bool condition = GetCondition();
        List<BaseValue> bodyValues = InterpretBodyNodes(ref condition);

        return bodyValues;
    }

    private List<BaseValue> InterpretBodyNodes(ref bool condition)
    {
        List<BaseValue> bodyValues = new();
        var i = 0;
        while (condition)
        {
            Logger.Log($"While Loop iteration {i++}", LogType.INFO);
            foreach (var node in whileLoopStatementNode.BodyNodes)
            {
                var bodyValue = InterpreterFactory.GetInterpreter(node).VisitNode();
                if (bodyValue.Count > 1) throw new Exception("Body value is not a single value");
                bodyValues.Add(bodyValue[0]);
            }
            var newConditionValueNode = InterpreterFactory.GetInterpreter(whileLoopStatementNode.ConditionNode).VisitSingleNode();
            condition = (int)newConditionValueNode.Value != 0;
        }

        return bodyValues;
    }

    private bool GetCondition()
    {
        var interpreter = InterpreterFactory.GetInterpreter(whileLoopStatementNode.ConditionNode);
        var conditionValue = interpreter.VisitSingleNode();

        if (conditionValue is not BooleanValue) throw new TypeConversionException(conditionValue.GetType(), typeof(BooleanValue));
        var conditionBoolean = (int)conditionValue.Value != 0;
        return conditionBoolean;
    }
}