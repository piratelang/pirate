using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PirateInterpreter.Values;

namespace PirateInterpreter.Interpreters;

public class ForLoopStatementNodeInterpreter : BaseInterpreter
{
    public IForLoopStatementNode Node { get; set; }

    public ForLoopStatementNodeInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger) : base(logger, InterpreterFactory)
    {
        if (node is not IForLoopStatementNode) throw new TypeConversionException(node.GetType(), typeof(IIfStatementNode));
        Node = (IForLoopStatementNode)node;

        Logger.Log($"Created {this.GetType().Name} : \"{Node.ToString()}\"", this.GetType().Name, Common.Enum.LogType.INFO);
    }
    public override BaseValue VisitNode()
    {
        var variablevalue = InterpreterFactory.GetInterpreter(Node.VariableNode, Logger).VisitNode();
        var startvalue = InterpreterFactory.GetInterpreter(Node.ValueNode, Logger).VisitNode();

        if (variablevalue is not Values.Variable) throw new TypeConversionException(variablevalue.GetType(), typeof(Values.Variable));
        if (startvalue is not Values.Integer) throw new TypeConversionException(startvalue.GetType(), typeof(Values.Integer));
        if (variablevalue.Value is not int) throw new TypeConversionException(variablevalue.Value.GetType(), typeof(Values.Integer));
        if (startvalue.Value is not int) throw new TypeConversionException(startvalue.Value.GetType(), typeof(int));


        var variable = (Values.Variable)variablevalue;
        var start = (int)startvalue.Value;

        List<BaseValue> bodyValues = new();
        for (int i = (int)variable.Value; i < (int)startvalue.Value; i++)
        {
            foreach (var node in Node.BodyNodes)
            {
                bodyValues.Add(InterpreterFactory.GetInterpreter(node, Logger).VisitNode());    
            }
        }

        return bodyValues.FirstOrDefault();
    }
}