using Pirate.Interpreter.Interfaces;
using Pirate.Interpreter.Values;
using Pirate.Interpreter.Interpreters.Interfaces;
using Pirate.Parser;

namespace Pirate.Interpreter;

/// <summary>
/// The starting point for the interpreter.
/// </summary>
public class Interpreter : IInterpreter
{
    private IInterpreterFactory InterpreterFactory;

    public ILogger Logger { get; set; }

    public Interpreter(ILogger logger, IInterpreterFactory interpreterFactory)
    {
        Logger = logger;
        InterpreterFactory = interpreterFactory;
    }

    public List<BaseValue> StartInterpreter(Scope scope)
    {
        List<BaseValue> result = new();
        foreach (var item in scope.Nodes)
        {
            Logger.Info($"Interpreting {item.GetType().Name}");
            var interpreter = InterpreterFactory.GetInterpreter(item);
            result.AddRange(interpreter.VisitNode());
        }
        return result;
    }
}