
using Pirate.Common.Interfaces;
using Pirate.Interpreter.Values;
using Pirate.Parser.Node;
using Pirate.Parser.Node.Interfaces;

namespace Pirate.Interpreter.Interpreters;

/// <summary>
/// Converts the variable declaration node to a variable value.
/// Also sets the variable value in the symbol table.
/// </summary>
public class VariableDeclarationInterpreter : BaseInterpreter
{
    public VariableDeclarationNode variableDeclarationNode { get; set; }

    public VariableDeclarationInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger) : base(logger, InterpreterFactory)
    {
        if (node is not VariableDeclarationNode) throw new TypeConversionException(node.GetType(), typeof(VariableDeclarationNode));
        variableDeclarationNode = (VariableDeclarationNode)node;

        Logger.Log($"Created {GetType().Name} : \"{variableDeclarationNode.ToString()}\"", LogType.INFO);
    }

    public override List<BaseValue> VisitNode()
    {
        Logger.Log($"Visiting {GetType().Name} : \"{variableDeclarationNode.ToString()}\"", LogType.INFO);
        if (variableDeclarationNode.Identifier.Value.Value is not string) throw new TypeConversionException(typeof(string));

        var Identifier = (string)variableDeclarationNode.Identifier.Value.Value;
        var interpreter = InterpreterFactory.GetInterpreter(variableDeclarationNode.Value);
        var result = interpreter.VisitSingleNode();

        SymbolTable.Instance(Logger).SetBaseValue(Identifier, result);

        var variable = new VariableValue(Identifier, Logger, InterpreterFactory);
        return new List<BaseValue> { variable };
    }
}