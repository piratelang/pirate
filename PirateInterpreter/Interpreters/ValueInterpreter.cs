using PirateInterpreter.Values;

namespace PirateInterpreter.Interpreters;

public class ValueInterpreter : BaseInterpreter
{
    private IValueNode valueNode { get; set; }
    public ValueInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger) : base(logger, InterpreterFactory)
    {
        if (node is not IValueNode) throw new TypeConversionException(node.GetType(), typeof(IValueNode));
        valueNode = (IValueNode)node;

        Logger.Log($"Created {this.GetType().Name} : \"{valueNode.ToString()}\"", Common.Enum.LogType.INFO);
    }

    public override List<BaseValue> VisitNode()
    {
        Logger.Log($"Visiting {this.GetType().Name} : \"{valueNode.ToString()}\"", Common.Enum.LogType.INFO);
        if (valueNode.Value.Value == null) throw new ArgumentNullException($"{valueNode.Value.GetType().Name} does not contain a vaild value type.");
        switch (valueNode.Value.TokenType)
        {
            case TokenValue.INT:
                return new List<BaseValue> { new IntegerValue(valueNode.Value.Value, Logger) };
            case TokenValue.STRING:
                return new List<BaseValue> { new Values.StringValue(valueNode.Value.Value, Logger) };
            case TokenValue.CHAR:
                return new List<BaseValue> { new Values.CharValue(valueNode.Value.Value, Logger) };
            case TokenValue.FLOAT:
                return new List<BaseValue> { new FloatValue(valueNode.Value.Value, Logger) };
            case TokenSyntax.IDENTIFIER:
                return new List<BaseValue> { new VariableValue((string)valueNode.Value.Value, Logger, InterpreterFactory) };
        } 
        throw new ArgumentNullException($"{valueNode.Value.GetType().Name} is not trecognized as a BaseValue type.");

    }
}