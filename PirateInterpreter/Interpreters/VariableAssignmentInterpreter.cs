using PirateInterpreter.Values;

namespace PirateInterpreter.Interpreters;

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
        if (variableAssignmentNode.Identifier.Value.Value is not string)
        {
            if (variableAssignmentNode.Identifier.Value.Value != null)
            {
                throw new TypeConversionException(variableAssignmentNode.Identifier.Value.Value.GetType(), typeof(string));
            }
            throw new TypeConversionException(typeof(string));
        }

        var identifier = (string)variableAssignmentNode.Identifier.Value.Value;
        SymbolTable.Instance(Logger).Set(identifier, variableAssignmentNode.Value);

        var variable = new Variable(identifier, Logger, InterpreterFactory);
        return new List<BaseValue> { variable };
    }
}