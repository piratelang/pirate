using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PirateInterpreter.Values;

namespace PirateInterpreter.Interpreters;

public class IfStatementInterpreter : BaseInterpreter
{
    public IIfStatementNode ifStatementNode { get; set; }

    public IfStatementInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger) : base(logger, InterpreterFactory)
    {
        if (node is not IIfStatementNode) throw new TypeConversionException(node.GetType(), typeof(IIfStatementNode));
        ifStatementNode = (IIfStatementNode)node;

        Logger.Log($"Created {this.GetType().Name} : \"{ifStatementNode.ToString()}\"", this.GetType().Name, Common.Enum.LogType.INFO);
    }
    public override List<BaseValue> VisitNode()
    {
        Logger.Log($"Visiting {this.GetType().Name} : \"{ifStatementNode.ToString()}\"", this.GetType().Name, Common.Enum.LogType.INFO);
        var interpreter = InterpreterFactory.GetInterpreter(ifStatementNode.ConditionNode, Logger);
        var conditionValue = interpreter.VisitSingleNode();

        if (conditionValue is not Values.Boolean) throw new TypeConversionException(conditionValue.GetType(), typeof(Values.Boolean));
        var conditionBoolean = (int)conditionValue.Value != 0;
        
        List<BaseValue> bodyValues = new();
        if (conditionBoolean && ifStatementNode.BodyNodes.Count > 0)
        {
            foreach (var node in ifStatementNode.BodyNodes)
            {
                var bodyValue = InterpreterFactory.GetInterpreter(node, Logger).VisitNode();
                if (bodyValue.Count > 1) throw new Exception("Body value is not a single value");
                bodyValues.Add(bodyValue[0]);
            }
        }

        if (!conditionBoolean && ifStatementNode.ElseNode is not null)
        {
            foreach (var node in ifStatementNode.ElseNode)
            {
                var bodyValue = InterpreterFactory.GetInterpreter(node, Logger).VisitNode();
                if (bodyValue.Count > 1) throw new Exception("Body value is not a single value");
                bodyValues.Add(bodyValue[0]);
            }
        }
        
        return bodyValues;
    }
}