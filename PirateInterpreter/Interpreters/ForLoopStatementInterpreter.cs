using PirateInterpreter.Values;

namespace PirateInterpreter.Interpreters;

public class ForLoopStatementInterpreter : BaseInterpreter
{
    public IForLoopStatementNode forLoopStatementNode { get; set; }

    public ForLoopStatementInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger) : base(logger, InterpreterFactory)
    {
        if (node is not IForLoopStatementNode) throw new TypeConversionException(node.GetType(), typeof(IIfStatementNode));
        forLoopStatementNode = (IForLoopStatementNode)node;

        Logger.Log($"Created {this.GetType().Name} : \"{forLoopStatementNode.ToString()}\"", LogType.INFO);
    }
    public override List<BaseValue> VisitNode()
    {
        Logger.Log($"Visiting {this.GetType().Name} : \"{forLoopStatementNode.ToString()}\"", LogType.INFO);

        var interpreter = InterpreterFactory.GetInterpreter(forLoopStatementNode.VariableNode);
        var variableValue = interpreter.VisitSingleNode();

        interpreter = InterpreterFactory.GetInterpreter(forLoopStatementNode.ValueNode);
        var startValue = interpreter.VisitSingleNode();

        if (variableValue is not Values.VariableValue) throw new TypeConversionException(variableValue.GetType(), typeof(Values.VariableValue));
        if (startValue is not Values.IntegerValue) throw new TypeConversionException(startValue.GetType(), typeof(Values.IntegerValue));
        if (variableValue.Value is not Int64 && variableValue.Value is not int) throw new TypeConversionException(variableValue.Value.GetType(),typeof(Int64));
        if (startValue.Value is not Int64 && startValue.Value is not int) throw new TypeConversionException(startValue.Value.GetType(), typeof(Int64));


        // var variable = (int)variableValue.Value;
        Int64.TryParse(variableValue.Value.ToString(), out Int64 variable);
        Int64.TryParse(startValue.Value.ToString(), out Int64 start);

        List<BaseValue> bodyValues = new();
        for (Int64 i = variable; i < start; i++)
        {
            Logger.Log($"For Loop iteration: {i}", LogType.INFO);
            foreach (var node in forLoopStatementNode.BodyNodes)
            {
                interpreter = InterpreterFactory.GetInterpreter(node);
                var bodyValue = interpreter.VisitSingleNode();
                bodyValues.Add(bodyValue);
            }

            SymbolTable.Instance(Logger).SetBaseValue((string)forLoopStatementNode.VariableNode.Identifier.Value.Value, new IntegerValue(i, Logger));
        }

        return bodyValues;
    }
}