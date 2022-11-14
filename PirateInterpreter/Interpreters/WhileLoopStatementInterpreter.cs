using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PirateInterpreter.Values;

namespace PirateInterpreter.Interpreters;

public class WhileLoopStatementInterpreter : BaseInterpreter
{
    public IWhileLoopStatementNode whileLoopStatementNode { get; set; }

    public WhileLoopStatementInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger) : base(logger, InterpreterFactory)
    {
        if (node is not IWhileLoopStatementNode) throw new TypeConversionException(node.GetType(), typeof(IIfStatementNode));
        whileLoopStatementNode = (IWhileLoopStatementNode)node;

        Logger.Log($"Created {this.GetType().Name} : \"{whileLoopStatementNode.ToString()}\"", this.GetType().Name, Common.Enum.LogType.INFO);
    }
    public override List<BaseValue> VisitNode()
    {
        var interpreter = InterpreterFactory.GetInterpreter(whileLoopStatementNode.ConditionNode, Logger);
        var conditionValue = interpreter.VisitSingleNode();

        if (conditionValue is not Values.Boolean) throw new TypeConversionException(conditionValue.GetType(), typeof(Values.Boolean));
        var conditionBoolean = (int)conditionValue.Value != 0;

        List<BaseValue> bodyValues = new();
        while (conditionBoolean)
        {
            foreach (var node in whileLoopStatementNode.BodyNodes)
            {
                var bodyValue = InterpreterFactory.GetInterpreter(node, Logger).VisitNode();
                if (bodyValue.Count > 1) throw new Exception("Body value is not a single value");
                bodyValues.Add(bodyValue[0]);
            }
            var newConditionValueNode = InterpreterFactory.GetInterpreter(whileLoopStatementNode.ConditionNode, Logger).VisitNode();
            conditionBoolean = (int)conditionValue.Value != 0;
        }

        return bodyValues;
    }
}