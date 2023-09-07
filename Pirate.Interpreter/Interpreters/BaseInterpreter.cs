using Pirate.Interpreter.Runtime;
using Pirate.Interpreter.Values;

namespace Pirate.Interpreter.Interpreters;

/// <summary>
/// This class is a base class for all interpreters.
/// </summary>
public abstract class BaseInterpreter
{
    protected readonly ILogger Logger;
    protected InterpreterFactory InterpreterFactory { get; private set; }

    public BaseInterpreter(ILogger logger, InterpreterFactory interpreterFactory)
    {
        Logger = logger;
        InterpreterFactory = interpreterFactory;
    }

    public abstract List<BaseValue> VisitNode();

    public BaseValue VisitSingleNode()
    {
        var node = VisitNode();
        if (node.Count == 0) return null;
        if (node.Count > 1 && node.Count < 0) throw new Exception("Value is not a single value");
        return node[0];
    }
}