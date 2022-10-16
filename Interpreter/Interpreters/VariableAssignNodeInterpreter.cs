using Interpreter.Interpreters.Interfaces;
using Parser.Node.Interfaces;
using Common;
using Interpreter.Values;
using Parser.Node;

namespace Interpreter.Interpreters;

public class VariableAssignNodeInterpreter : BaseInterpreter
{
    public VariableAssignNode Node { get; set; }
    private InterpreterFactory interpreterFactory;
    public ILogger Logger { get; set; }

    public VariableAssignNodeInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger)
    {
        Node = node as VariableAssignNode;
        interpreterFactory = InterpreterFactory;
        Logger = logger;
        Logger.Log($"Created {this.GetType().Name} : \"{Node.ToString()}\"", this.GetType().Name, Common.Enum.LogType.INFO);
    }

    public override BaseValue VisitNode()
    {
        var Identifier = Node.Identifier.Value.Value;
        

        SymbolTable.Instance(Logger).Set((string)Identifier, Node.Value);

        var variable = new Variable((string)Identifier, Logger);

        return variable;
    }
}