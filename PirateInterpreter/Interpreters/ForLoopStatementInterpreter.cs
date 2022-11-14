using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PirateInterpreter.Values;

namespace PirateInterpreter.Interpreters;

public class ForLoopStatementInterpreter : BaseInterpreter
{
    public IForLoopStatementNode Node { get; set; }

    public ForLoopStatementInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger) : base(logger, InterpreterFactory)
    {
        if (node is not IForLoopStatementNode) throw new TypeConversionException(node.GetType(), typeof(IIfStatementNode));
        Node = (IForLoopStatementNode)node;

        Logger.Log($"Created {this.GetType().Name} : \"{Node.ToString()}\"", this.GetType().Name, Common.Enum.LogType.INFO);
    }
    public override List<BaseValue> VisitNode()
    {
        var variableValueNode = InterpreterFactory.GetInterpreter(Node.VariableNode, Logger).VisitNode();
        if (variableValueNode.Count > 1) throw new Exception("Variable value is not a single value");
        var variableValue = variableValueNode[0];

        var startValueNode = InterpreterFactory.GetInterpreter(Node.ValueNode, Logger).VisitNode();
        if (startValueNode.Count > 1) throw new Exception("Start value is not a single value");
        var startValue = startValueNode[0];

        if (variableValue is not Values.Variable) throw new TypeConversionException(variableValue.GetType(), typeof(Values.Variable));
        if (startValue is not Values.Integer) throw new TypeConversionException(startValue.GetType(), typeof(Values.Integer));
        if (variableValue.Value is not int) throw new TypeConversionException(variableValue.Value.GetType(), typeof(Values.Integer));
        if (startValue.Value is not int) throw new TypeConversionException(startValue.Value.GetType(), typeof(int));


        var variable = (Values.Variable)variableValue;
        var start = (int)startValue.Value;

        List<BaseValue> bodyValues = new();
        for (int i = (int)variable.Value; i < (int)startValue.Value; i++)
        {
            foreach (var node in Node.BodyNodes)
            {
                var bodyValue = InterpreterFactory.GetInterpreter(node, Logger).VisitNode();
                if (bodyValue.Count > 1) throw new Exception("Body value is not a single value");
                bodyValues.Add(bodyValue[0]);
            }
        }

        return bodyValues;
    }
}