using Pirate.Interpreter.Interfaces;
using Pirate.Interpreter.Values;

namespace Pirate.Interpreter.Interpreters;

/// <summary>
/// Converts a ValueNode into the corresponding BaseValue
/// </summary>
/// eg. a ValueNode with the Value of 32 becomes a list of BaseValue with one IntegerValue with the value of 32

public class ValueInterpreter : BaseInterpreter
{
    private IValueNode _valueNode { get; set; }
    private IRuntime _runtime { get; set; }

    public ValueInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger, IRuntime runtime) : base(logger, InterpreterFactory)
    {
        if (node is not IValueNode) throw new TypeConversionException(node.GetType(), typeof(IValueNode));
        _valueNode = (IValueNode)node;
        _runtime = runtime;

        Logger.Log($"Created {GetType().Name} : \"{_valueNode.ToString()}\"", LogType.INFO);
    }

    public override List<BaseValue> VisitNode()
    {
        Logger.Log($"Visiting {GetType().Name} : \"{_valueNode.ToString()}\"", LogType.INFO);
        if (_valueNode.Value.Value == null) throw new ArgumentNullException($"{_valueNode.Value.GetType().Name} does not contain a vaild value type.");
        switch (_valueNode.Value.TokenType)
        {
            case TokenType.INT:
                return new List<BaseValue> { new IntegerValue(_valueNode.Value.Value, Logger) };
            case TokenType.STRING:
                return new List<BaseValue> { new StringValue(_valueNode.Value.Value, Logger) };
            case TokenType.CHAR:
                return new List<BaseValue> { new CharValue(_valueNode.Value.Value, Logger) };
            case TokenType.FLOAT:
                return new List<BaseValue> { new FloatValue(_valueNode.Value.Value, Logger) };
            case TokenType.IDENTIFIER:
                return new List<BaseValue> { new VariableValue((string)_valueNode.Value.Value, Logger, _runtime) };
        }
        throw new ArgumentNullException($"{_valueNode.Value.GetType().Name} is not a recognized as a BaseValue type.");

    }
}