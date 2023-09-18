using Pirate.Interpreter.Interfaces;
using Pirate.Interpreter.Values;

namespace Pirate.Interpreter.Interpreters;

/// <summary>
/// Interprets the for loop statement and gets the value from the body nodes.
/// </summary>
public class ForLoopStatementInterpreter : BaseInterpreter
{
    public IForLoopStatementNode forLoopStatementNode { get; set; }
    private IRuntime _runtime { get; set; }

    public ForLoopStatementInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger, IRuntime runtime) : base(logger, InterpreterFactory)
    {
        if (node is not IForLoopStatementNode) throw new TypeConversionException(node.GetType(), typeof(IIfStatementNode));
        forLoopStatementNode = (IForLoopStatementNode)node;
        _runtime = runtime;

        Logger.Info($"Created {GetType().Name} : \"{forLoopStatementNode.ToString()}\"");
    }
    public override List<BaseValue> VisitNode()
    {
        Logger.Info($"Visiting {GetType().Name} : \"{forLoopStatementNode.ToString()}\"");

        var interpreter = InterpreterFactory.GetInterpreter(forLoopStatementNode.VariableNode);
        var variableValue = interpreter.VisitSingleNode();

        interpreter = InterpreterFactory.GetInterpreter(forLoopStatementNode.ValueNode);
        var startValue = interpreter.VisitSingleNode();

        if (variableValue is not VariableValue) throw new TypeConversionException(variableValue.GetType(), typeof(VariableValue));
        if (startValue is not IntegerValue) throw new TypeConversionException(startValue.GetType(), typeof(IntegerValue));
        if (variableValue.Value is not long && variableValue.Value is not int) throw new TypeConversionException(variableValue.Value.GetType(), typeof(long));
        if (startValue.Value is not long && startValue.Value is not int) throw new TypeConversionException(startValue.Value.GetType(), typeof(long));

        long.TryParse(variableValue.Value.ToString(), out long variable);
        long.TryParse(startValue.Value.ToString(), out long start);

        List<BaseValue> bodyValues = InterpretBodyNodes(ref interpreter, variable, start);

        return bodyValues;
    }

    private List<BaseValue> InterpretBodyNodes(ref BaseInterpreter interpreter, long variable, long start)
    {
        List<BaseValue> bodyValues = new();
        for (long i = variable; i < start; i++)
        {
            Logger.Info($"For Loop iteration: {i}");
            foreach (var node in forLoopStatementNode.BodyNodes)
            {
                interpreter = InterpreterFactory.GetInterpreter(node);
                var bodyValue = interpreter.VisitSingleNode();
                bodyValues.Add(bodyValue);
            }

           _runtime.Variables.Set((string)forLoopStatementNode.VariableNode.Identifier.Value.Value, new IntegerValue(i, Logger));
        }

        return bodyValues;
    }
}