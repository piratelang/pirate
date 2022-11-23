using PirateInterpreter.Values;

namespace PirateInterpreter.Interpreters;

public class ForLoopStatementInterpreter : BaseInterpreter
{
    public IForLoopStatementNode forLoopStatementNode { get; set; }

    public ForLoopStatementInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger) : base(logger, InterpreterFactory)
    {
        if (node is not IForLoopStatementNode) throw new TypeConversionException(node.GetType(), typeof(IIfStatementNode));
        forLoopStatementNode = (IForLoopStatementNode)node;

        Logger.Log($"Created {this.GetType().Name} : \"{forLoopStatementNode.ToString()}\"", Common.Enum.LogType.INFO);
    }
    public override List<BaseValue> VisitNode()
    {
        Logger.Log($"Visiting {this.GetType().Name} : \"{forLoopStatementNode.ToString()}\"", Common.Enum.LogType.INFO);
        var interpreter = InterpreterFactory.GetInterpreter(forLoopStatementNode.VariableNode);
        var variableValue = interpreter.VisitSingleNode();

        interpreter = InterpreterFactory.GetInterpreter(forLoopStatementNode.ValueNode);
        var startValue = interpreter.VisitSingleNode();

        if (variableValue is not Values.VariableValue) throw new TypeConversionException(variableValue.GetType(), typeof(Values.VariableValue));
        if (startValue is not Values.IntegerValue) throw new TypeConversionException(startValue.GetType(), typeof(Values.IntegerValue));
        if (variableValue.Value is not Int64 && variableValue.Value is not int) throw new TypeConversionException(variableValue.Value.GetType(),typeof(int));
        if (startValue.Value is not Int64 && startValue.Value is not int) throw new TypeConversionException(startValue.Value.GetType(), typeof(int));


        // var variable = (int)variableValue.Value;
        int.TryParse(variableValue.Value.ToString(), out int variable);
        int.TryParse(startValue.Value.ToString(), out int start);

        List<BaseValue> bodyValues = new();
        for (int i = variable; i < start; i++)
        {
            Logger.Log($"For Loop iteration: {i}", Common.Enum.LogType.INFO);
            foreach (var node in forLoopStatementNode.BodyNodes)
            {
                var bodyValue = InterpreterFactory.GetInterpreter(node).VisitSingleNode();
                bodyValues.Add(bodyValue);
            }

            SymbolTable.Instance(Logger).SetBaseValue((string)forLoopStatementNode.VariableNode.Identifier.Value.Value, new IntegerValue(i, Logger));
        }

        return bodyValues;
    }
}