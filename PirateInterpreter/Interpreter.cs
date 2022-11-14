using PirateInterpreter.Interpreters;
using PirateInterpreter.Values;
using PirateInterpreter.Interfaces;
using PirateParser;

namespace PirateInterpreter;

public class Interpreter : IInterpreter
{
    private IObjectSerializer ObjectSerializer;
    public ILogger Logger { get; set; }
    public Interpreter(IObjectSerializer objectSerializer, ILogger logger)
    {
        ObjectSerializer = objectSerializer;
        Logger = logger;
    }

    public List<BaseValue> StartInterpreter(string filename)
    {
        if (filename == null)
        {
            throw new NullReferenceException("filename provided is null");
        }
        var scopeList = ObjectSerializer.Deserialize<Scope>(filename + ".pirate");

        List<BaseValue> result = new();
        var interpreterFactory = new InterpreterFactory();
        foreach (var item in scopeList.Nodes)
        {
            Logger.Log($"Interpreting {item.GetType().Name}", this.GetType().Name, LogType.INFO);
            var interpreter = interpreterFactory.GetInterpreter(item, Logger);
            result.AddRange(interpreter.VisitNode());
        }
        return result;
    }
}