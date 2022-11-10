using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PirateInterpreter.Values;

namespace PirateInterpreter.Interpreters;

public class IfStatementNodeInterpreter : BaseInterpreter
{
    public IIfStatementNode Node { get; set; }

    public IfStatementNodeInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger) : base(logger, InterpreterFactory)
    {
        if (node is not IIfStatementNode) throw new TypeConversionException(node.GetType(), typeof(IIfStatementNode));
        Node = (IIfStatementNode)node;

        Logger.Log($"Created {this.GetType().Name} : \"{Node.ToString()}\"", this.GetType().Name, Common.Enum.LogType.INFO);
    }
    public override BaseValue VisitNode()
    {
        var conditionValue = InterpreterFactory.GetInterpreter(Node.ConditionNode, Logger).VisitNode();

        if (conditionValue is not Values.Boolean) throw new TypeConversionException(conditionValue.GetType(), typeof(Values.Boolean));
        var conditionBoolean = (int)conditionValue.Value != 0;
        
        List<BaseValue> bodyValues = new();
        if (conditionBoolean && Node.BodyNodes.Count > 0)
        {
            foreach (var node in Node.BodyNodes)
            {
                bodyValues.Add(InterpreterFactory.GetInterpreter(node, Logger).VisitNode());
            }
        }

        
        return bodyValues.FirstOrDefault();
    }
}