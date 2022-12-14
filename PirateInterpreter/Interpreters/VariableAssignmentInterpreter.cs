using PirateInterpreter.Values;

namespace PirateInterpreter.Interpreters;

/// <summary>
/// Converts the variable assignment node to a variable value.
/// Also sets the variable value in the symbol table.
/// </summary>
public class VariableAssignmentInterpreter : BaseInterpreter
{
    public VariableAssignmentNode variableAssignmentNode { get; private set; }

    public VariableAssignmentInterpreter(INode node, ILogger Logger, InterpreterFactory InterpreterFactory) : base(Logger, InterpreterFactory)
    {
        if (node is not VariableAssignmentNode) throw new TypeConversionException(node.GetType(), typeof(VariableAssignmentNode));
        variableAssignmentNode = (VariableAssignmentNode)node;
    }

    public override List<BaseValue> VisitNode()
    {
        Logger.Log($"Visiting {this.GetType().Name} : \"{variableAssignmentNode.ToString()}\"", LogType.INFO);
        if (variableAssignmentNode.Identifier.Value.Value is not string) throw new TypeConversionException(typeof(string));
        var identifier = (string)variableAssignmentNode.Identifier.Value.Value;

        var interpreter = InterpreterFactory.GetInterpreter(variableAssignmentNode.Value);
        var result = interpreter.VisitSingleNode();

        SymbolTable.Instance(Logger).SetBaseValue(identifier, result);

        var variable = new VariableValue(identifier, Logger, InterpreterFactory);
        return new List<BaseValue> { variable };
    }
}