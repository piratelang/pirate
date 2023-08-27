using Pirate.Common.Enum;
using Pirate.Common.Errors;
using Pirate.Common.Interfaces;
using Pirate.Interpreter.Values;
using Pirate.Parser.Node.Interfaces;

namespace Pirate.Interpreter.Interpreters;

/// <summary>
/// Based on the condition, this interpreter will execute the true or false branch.
/// </summary>
public class IfStatementInterpreter : BaseInterpreter
{
    public IIfStatementNode ifStatementNode { get; set; }

    public IfStatementInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger) : base(logger, InterpreterFactory)
    {
        if (node is not IIfStatementNode) throw new TypeConversionException(node.GetType(), typeof(IIfStatementNode));
        ifStatementNode = (IIfStatementNode)node;

        Logger.Log($"Created {GetType().Name} : \"{ifStatementNode.ToString()}\"", LogType.INFO);
    }
    public override List<BaseValue> VisitNode()
    {
        Logger.Log($"Visiting {GetType().Name} : \"{ifStatementNode.ToString()}\"", LogType.INFO);
        var interpreter = InterpreterFactory.GetInterpreter(ifStatementNode.ConditionNode);
        var conditionValue = interpreter.VisitSingleNode();

        if (conditionValue is not BooleanValue) throw new TypeConversionException(conditionValue.GetType(), typeof(BooleanValue));
        var conditionBoolean = (int)conditionValue.Value != 0;

        List<BaseValue> resultValues = new();
        if (conditionBoolean && ifStatementNode.BodyNodes.Count > 0) InterpretBodyNodes(resultValues, ifStatementNode.BodyNodes);
        if (!conditionBoolean && ifStatementNode.ElseNode is not null) InterpretBodyNodes(resultValues, ifStatementNode.ElseNode);

        return resultValues;
    }

    private void InterpretBodyNodes(List<BaseValue> resultValues, List<INode> bodyNodes)
    {
        foreach (var node in bodyNodes)
        {
            var bodyValue = InterpreterFactory.GetInterpreter(node).VisitNode();
            if (bodyValue.Count > 1) throw new Exception("Body value is not a single value");
            resultValues.Add(bodyValue[0]);
        }
    }
}