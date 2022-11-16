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
        Logger.Log($"Visiting {this.GetType().Name} : \"{variableAssignmentNode.ToString()}\"", this.GetType().Name, Common.Enum.LogType.INFO);
        if (variableAssignmentNode.Identifier.Value.Value is not string)
        {
            if (variableAssignmentNode.Identifier.Value.Value != null)
            {
                throw new TypeConversionException(variableAssignmentNode.Identifier.Value.Value.GetType(), typeof(string));
            }
            throw new TypeConversionException(typeof(string));
        }

        var identifier = (string)variableAssignmentNode.Identifier.Value.Value;
        var interpreter = _interpreterFactory.GetInterpreter(variableAssignmentNode.Value, Logger);
        var result = interpreter.VisitSingleNode();

        SymbolTable.Instance(Logger).Set(identifier, result);

        var variable = new Variable(identifier, Logger, _interpreterFactory);
        return new List<BaseValue> { variable };
    }
}