using Pirate.Interpreter.Interfaces;
using Pirate.Interpreter.Values;

namespace Pirate.Interpreter.Interpreters;

/// <summary>
/// Converts the variable declaration node to a variable value.
/// Also sets the variable value in the symbol table.
/// </summary>
public class VariableDeclarationInterpreter : BaseInterpreter
{
    public VariableDeclarationNode variableDeclarationNode { get; set; }
    private IRuntime _runtime { get; set; }

    public VariableDeclarationInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger, IRuntime runtime) : base(logger, InterpreterFactory)
    {
        if (node is not VariableDeclarationNode) throw new TypeConversionException(node.GetType(), typeof(VariableDeclarationNode));
        variableDeclarationNode = (VariableDeclarationNode)node;

        Logger.Info($"Created {GetType().Name} : \"{variableDeclarationNode.ToString()}\"");
        _runtime = runtime;
    }

    public override List<BaseValue> VisitNode()
    {
        Logger.Info($"Visiting {GetType().Name} : \"{variableDeclarationNode.ToString()}\"");
        if (variableDeclarationNode.Identifier.Value.Value is not string) throw new TypeConversionException(typeof(string));

        var Identifier = (string)variableDeclarationNode.Identifier.Value.Value;
        var interpreter = InterpreterFactory.GetInterpreter(variableDeclarationNode.Value);
        var result = interpreter.VisitSingleNode();

        _runtime.Variables.Set(Identifier, result);

        var variable = new VariableValue(Identifier, Logger, _runtime);
        return new List<BaseValue> { variable };
    }
}