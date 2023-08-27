
using Pirate.Common.Interfaces;
using Pirate.Interpreter;
using Pirate.Interpreter.Values;
using Pirate.Parser.Node.Interfaces;

namespace Pirate.Interpreter.Interpreters;

/// <summary>
/// Interprets the for loop statement and gets the value from the body nodes.
/// </summary>
public class ForLoopStatementInterpreter : BaseInterpreter
{
    public IForLoopStatementNode forLoopStatementNode { get; set; }

    public ForLoopStatementInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger) : base(logger, InterpreterFactory)
    {
        if (node is not IForLoopStatementNode) throw new TypeConversionException(node.GetType(), typeof(IIfStatementNode));
        forLoopStatementNode = (IForLoopStatementNode)node;

        Logger.Log($"Created {GetType().Name} : \"{forLoopStatementNode.ToString()}\"", LogType.INFO);
    }
    public override List<BaseValue> VisitNode()
    {
        Logger.Log($"Visiting {GetType().Name} : \"{forLoopStatementNode.ToString()}\"", LogType.INFO);

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