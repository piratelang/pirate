using PirateInterpreter.Values;

namespace PirateInterpreter.Interpreters;

public class VariableDeclarationInterpreter : BaseInterpreter
{
    public VariableDeclarationNode Node { get; set; }

    public VariableDeclarationInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger) : base(logger, InterpreterFactory)
    {
        if (node is not VariableDeclarationNode)throw new TypeConversionException(node.GetType(), typeof(VariableDeclarationNode));
        Node = (VariableDeclarationNode)node;

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