using PirateInterpreter.Values;

namespace PirateInterpreter.Interpreters;

public class VariableDeclarationInterpreter : BaseInterpreter
{
    public VariableDeclarationNode Node { get; set; }

    public VariableDeclarationInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger) : base(logger, InterpreterFactory)
    {
        if (node is not VariableDeclarationNode)throw new TypeConversionException(node.GetType(), typeof(VariableDeclarationNode));
        Node = (VariableDeclarationNode)node;

        Logger.Log($"Created {this.GetType().Name} : \"{variableAssignNode.ToString()}\"", this.GetType().Name, Common.Enum.LogType.INFO);
    }

    public override List<BaseValue> VisitNode()
    {
        if (variableAssignNode.Identifier.Value.Value is not string)
        {
            if (variableAssignNode.Identifier.Value.Value != null)
            {
                throw new TypeConversionException(variableAssignNode.Identifier.Value.Value.GetType(), typeof(string));
            }
            throw new TypeConversionException(typeof(string));
        }

        var Identifier = (string)variableAssignNode.Identifier.Value.Value;
        SymbolTable.Instance(Logger).Set(Identifier, variableAssignNode.Value);

        var variable = new Variable(Identifier, Logger, InterpreterFactory);
        return new List<BaseValue> { variable };
    }
}