using Pirate.Interpreter.Values;

namespace Pirate.Interpreter.Interpreters;

/// <summary>
/// Converts a ValueNode into the corresponding BaseValue
/// </summary>
/// eg. a ValueNode with the Value of 32 becomes a list of BaseValue with one IntegerValue with the value of 32

public class ValueInterpreter : BaseInterpreter
{
    private IValueNode valueNode { get; set; }

    public ValueInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger) : base(logger, InterpreterFactory)
    {
        if (node is not IValueNode) throw new TypeConversionException(node.GetType(), typeof(IValueNode));
        valueNode = (IValueNode)node;

        Logger.Log($"Created {GetType().Name} : \"{valueNode.ToString()}\"", LogType.INFO);
    }

    public override List<BaseValue> VisitNode()
    {
        Logger.Log($"Visiting {GetType().Name} : \"{valueNode.ToString()}\"", LogType.INFO);
        if (valueNode.Value.Value == null) throw new ArgumentNullException($"{valueNode.Value.GetType().Name} does not contain a vaild value type.");
        switch (valueNode.Value.TokenType)
        {
            case TokenType.INT:
                return new List<BaseValue> { new IntegerValue(valueNode.Value.Value, Logger) };
            case TokenType.STRING:
                return new List<BaseValue> { new StringValue(valueNode.Value.Value, Logger) };
            case TokenType.CHAR:
                return new List<BaseValue> { new CharValue(valueNode.Value.Value, Logger) };
            case TokenType.FLOAT:
                return new List<BaseValue> { new FloatValue(valueNode.Value.Value, Logger) };
            case TokenType.IDENTIFIER:
                return new List<BaseValue> { new VariableValue((string)valueNode.Value.Value, Logger, InterpreterFactory) };
        }
        throw new ArgumentNullException($"{valueNode.Value.GetType().Name} is not a recognized as a BaseValue type.");

    }
}