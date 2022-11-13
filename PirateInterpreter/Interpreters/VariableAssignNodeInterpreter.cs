using PirateInterpreter.Values;

namespace PirateInterpreter.Interpreters;

public class VariableAssignNodeInterpreter : BaseInterpreter
{
    public VariableAssignNode Node { get; set; }

    public VariableAssignNodeInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger) : base(logger, InterpreterFactory)
    {
        if (node is not VariableAssignNode)throw new TypeConversionException(node.GetType(), typeof(VariableAssignNode));
        Node = (VariableAssignNode)node;

        Logger.Log($"Created {this.GetType().Name} : \"{Node.ToString()}\"", this.GetType().Name, Common.Enum.LogType.INFO);
    }

    public override List<BaseValue> VisitNode()
    {
        if (Node.Identifier.Value.Value is not string)
        {
            if (Node.Identifier.Value.Value != null)
            {
                throw new TypeConversionException(Node.Identifier.Value.Value.GetType(), typeof(string));
            }
            throw new TypeConversionException(typeof(string));
        }

        var Identifier = (string)Node.Identifier.Value.Value;
        SymbolTable.Instance(Logger).Set(Identifier, Node.Value);

        var variable = new Variable(Identifier, Logger, InterpreterFactory);
        return new List<BaseValue> { variable };
    }
}