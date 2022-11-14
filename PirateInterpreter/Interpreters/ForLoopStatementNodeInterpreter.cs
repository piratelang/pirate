using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PirateInterpreter.Values;

namespace PirateInterpreter.Interpreters;

public class ForLoopStatementNodeInterpreter : BaseInterpreter
{
    public IForLoopStatementNode forLoopStatementNode { get; set; }

    public ForLoopStatementNodeInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger) : base(logger, InterpreterFactory)
    {
        if (node is not IForLoopStatementNode) throw new TypeConversionException(node.GetType(), typeof(IIfStatementNode));
        forLoopStatementNode = (IForLoopStatementNode)node;

        Logger.Log($"Created {this.GetType().Name} : \"{forLoopStatementNode.ToString()}\"", this.GetType().Name, Common.Enum.LogType.INFO);
    }
    public override List<BaseValue> VisitNode()
    {
        var interpreter = InterpreterFactory.GetInterpreter(forLoopStatementNode.VariableNode, Logger);
        var variableValue = interpreter.VisitSingleNode();

        interpreter = InterpreterFactory.GetInterpreter(forLoopStatementNode.ValueNode, Logger);
        var startValue = interpreter.VisitSingleNode();

        if (variableValue is not Values.Variable) throw new TypeConversionException(variableValue.GetType(), typeof(Values.Variable));
        if (startValue is not Values.Integer) throw new TypeConversionException(startValue.GetType(), typeof(Values.Integer));
        if (variableValue.Value is not int) throw new TypeConversionException(variableValue.Value.GetType(), typeof(Values.Integer));
        if (startValue.Value is not int) throw new TypeConversionException(startValue.Value.GetType(), typeof(int));


        var variable = (int)variableValue.Value;
        var start = (int)startValue.Value;

        List<BaseValue> bodyValues = new();
        for (int i = variable; i < start; i++)
        {
            foreach (var node in forLoopStatementNode.BodyNodes)
            {
                var bodyValue = InterpreterFactory.GetInterpreter(node, Logger).VisitNode();
                if (bodyValue.Count > 1) throw new Exception("Body value is not a single value");
                bodyValues.Add(bodyValue[0]);
            }
        }

        return bodyValues;
    }
}