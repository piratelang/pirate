using PirateInterpreter.Values;

namespace PirateInterpreter.Interpreters;

public class ValueNodeInterpreter : BaseInterpreter
{
    private IValueNode valueNode { get; set; }
    public ValueNodeInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger) : base(logger, InterpreterFactory)
    {
        if (node is not IValueNode) throw new TypeConversionException(node.GetType(), typeof(IValueNode));
        valueNode = (IValueNode)node;

        Logger.Log($"Created {this.GetType().Name} : \"{valueNode.ToString()}\"", this.GetType().Name, Common.Enum.LogType.INFO);
    }

    public override List<BaseValue> VisitNode()
    {
        if (valueNode.Value.Value == null) throw new ArgumentNullException($"{valueNode.Value.GetType().Name} does not contain a vaild value type.");
        switch (valueNode.Value.TokenType)
        {
            case TokenValue.INT:
                return new List<BaseValue> { new Integer(valueNode.Value.Value, Logger) };
            case TokenValue.STRING:
                return new List<BaseValue> { new Values.String(valueNode.Value.Value, Logger) };
            case TokenValue.CHAR:
                return new List<BaseValue> { new Values.Char(valueNode.Value.Value, Logger) };
            case TokenValue.FLOAT:
                return new List<BaseValue> { new Float(valueNode.Value.Value, Logger) };
            case TokenSyntax.IDENTIFIER:
                return new List<BaseValue> { new Variable((string)valueNode.Value.Value, Logger, InterpreterFactory) };
        } 
        throw new ArgumentNullException($"{valueNode.Value.GetType().Name} is not trecognized as a BaseValue type.");

    }
}