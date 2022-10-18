using Interpreter.Interpreters.Interfaces;
using Parser.Node.Interfaces;
using Common;
using Interpreter.Values;
using Parser.Node;
using Common.Errors;

namespace Interpreter.Interpreters;

public class VariableAssignNodeInterpreter : BaseInterpreter
{
    public VariableAssignNode Node { get; set; }
    private InterpreterFactory interpreterFactory;
    public ILogger Logger { get; set; }

    public VariableAssignNodeInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger)
    {
        if (node is not VariableAssignNode)
        {
            throw new TypeConversionException(node.GetType(), typeof(VariableAssignNode));
        }
        Node = (VariableAssignNode)node;

        interpreterFactory = InterpreterFactory;
        Logger = logger;
        Logger.Log($"Created {this.GetType().Name} : \"{Node.ToString()}\"", this.GetType().Name, Common.Enum.LogType.INFO);
    }

    public override BaseValue VisitNode()
    {
        var Identifier = (string)Node.Identifier.Value.Value;
        if (Identifier is not string || Identifier == null)
        {
            throw new TypeConversionException(Node.Identifier.Value.Value.GetType(), typeof(string));
        }
        SymbolTable.Instance(Logger).Set(Identifier, Node.Value);

        var variable = new Variable(Identifier, Logger);
        return variable;
    }
}