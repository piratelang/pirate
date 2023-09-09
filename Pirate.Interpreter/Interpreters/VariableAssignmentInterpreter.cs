using Pirate.Interpreter.Runtime;
using Pirate.Interpreter.Values;

namespace Pirate.Interpreter.Interpreters;

/// <summary>
/// Converts the variable assignment node to a variable value.
/// Also sets the variable value in the symbol table.
/// </summary>
public class VariableAssignmentInterpreter : BaseInterpreter
{
    public VariableAssignmentNode variableAssignmentNode { get; private set; }

    private IRuntime _runtime { get; set; }

    public VariableAssignmentInterpreter(INode node, ILogger Logger, InterpreterFactory InterpreterFactory, IRuntime runtime) : base(Logger, InterpreterFactory)
    {
        if (node is not VariableAssignmentNode) throw new TypeConversionException(node.GetType(), typeof(VariableAssignmentNode));
        variableAssignmentNode = (VariableAssignmentNode)node;
        _runtime = runtime;
    }

    public override List<BaseValue> VisitNode()
    {
        Logger.Log($"Visiting {GetType().Name} : \"{variableAssignmentNode.ToString()}\"", LogType.INFO);
        if (variableAssignmentNode.Identifier.Value.Value is not string) throw new TypeConversionException(typeof(string));
        var identifier = (string)variableAssignmentNode.Identifier.Value.Value;

        var interpreter = InterpreterFactory.GetInterpreter(variableAssignmentNode.Value);
        var result = interpreter.VisitSingleNode();

        _runtime.Variables.Set(identifier, result);

        var variable = new VariableValue(identifier, Logger, _runtime);
        return new List<BaseValue> { variable };
    }
}