using PirateInterpreter.Values;

namespace PirateInterpreter.Interpreters;

public class VariableDeclarationInterpreter : BaseInterpreter
{
    public VariableDeclarationNode variableDeclarationNode { get; set; }

    public VariableDeclarationInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger) : base(logger, InterpreterFactory)
    {
        if (node is not VariableDeclarationNode)throw new TypeConversionException(node.GetType(), typeof(VariableDeclarationNode));
        variableDeclarationNode = (VariableDeclarationNode)node;

        Logger.Log($"Created {this.GetType().Name} : \"{variableDeclarationNode.ToString()}\"", this.GetType().Name, Common.Enum.LogType.INFO);
    }

    public override List<BaseValue> VisitNode()
    {
        Logger.Log($"Visiting {this.GetType().Name} : \"{variableDeclarationNode.ToString()}\"", this.GetType().Name, Common.Enum.LogType.INFO);
        if (variableDeclarationNode.Identifier.Value.Value is not string)
        {
            if (variableDeclarationNode.Identifier.Value.Value != null)
            {
                throw new TypeConversionException(variableDeclarationNode.Identifier.Value.Value.GetType(), typeof(string));
            }
            throw new TypeConversionException(typeof(string));
        }

        var Identifier = (string)variableDeclarationNode.Identifier.Value.Value;
        var interpreter = InterpreterFactory.GetInterpreter(variableDeclarationNode.Value, Logger);
        var result = interpreter.VisitSingleNode();

        SymbolTable.Instance(Logger).SetBaseValue(Identifier, result);

        var variable = new VariableValue(Identifier, Logger, InterpreterFactory);
        return new List<BaseValue> { variable };
    }
}