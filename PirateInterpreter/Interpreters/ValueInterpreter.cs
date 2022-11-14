using PirateInterpreter.Values;

namespace PirateInterpreter.Interpreters;

public class ValueInterpreter : BaseInterpreter
{
    private IValueNode Node { get; set; }
    public ValueInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger) : base(logger, InterpreterFactory)
    {
        if (node is not IValueNode) throw new TypeConversionException(node.GetType(), typeof(IValueNode));
        Node = (IValueNode)node;

        Logger.Log($"Created {this.GetType().Name} : \"{Node.ToString()}\"", this.GetType().Name, Common.Enum.LogType.INFO);
    }

    public override List<BaseValue> VisitNode()
    {
        if (Node.Value.Value == null) throw new ArgumentNullException($"{Node.Value.GetType().Name} does not contain a vaild value type.");
        switch (Node.Value.TokenType)
        {
            case TokenValue.INT:
                return new List<BaseValue> { new Integer(Node.Value.Value, Logger) };
            case TokenValue.STRING:
                return new List<BaseValue> { new Values.String(Node.Value.Value, Logger) };
            case TokenValue.CHAR:
                return new List<BaseValue> { new Values.Char(Node.Value.Value, Logger) };
            case TokenValue.FLOAT:
                return new List<BaseValue> { new Float(Node.Value.Value, Logger) };
            case TokenSyntax.IDENTIFIER:
                return new List<BaseValue> { new Variable((string)Node.Value.Value, Logger, InterpreterFactory) };
        } 
        throw new ArgumentNullException($"{Node.Value.GetType().Name} is not trecognized as a BaseValue type.");

    }
}