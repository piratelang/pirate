using PirateInterpreter.Values;
using PirateInterpreter.Interfaces;
using PirateParser;
using PirateInterpreter.Interpreters.Interfaces;

namespace PirateInterpreter;

/// <summary>
/// The starting point for the interpreter.
/// </summary>
public class Interpreter : IInterpreter
{
    private IObjectSerializer ObjectSerializer;
    private IInterpreterFactory InterpreterFactory;

    public ILogger Logger { get; set; }

    public Interpreter(IObjectSerializer objectSerializer, ILogger logger, IInterpreterFactory interpreterFactory)
    {
        ObjectSerializer = objectSerializer;
        Logger = logger;
        InterpreterFactory = interpreterFactory;
    }

    public List<BaseValue> StartInterpreter(string filename)
    {
        if (filename == null)
        {
            throw new NullReferenceException("filename provided is null");
        }
        var scopeList = ObjectSerializer.Deserialize<Scope>(filename + ".pirate");

        List<BaseValue> result = new();
        foreach (var item in scopeList.Nodes)
        {
            Logger.Log($"Interpreting {item.GetType().Name}", LogType.INFO);
            var interpreter = InterpreterFactory.GetInterpreter(item);
            result.AddRange(interpreter.VisitNode());
        }
        return result;
    }
}