using System.Runtime.Serialization;
using Interpreter.Interpreters;
using Interpreter.Values;
using Parser;
using Common.Interfaces;
using Common.Enum;

namespace Interpreter;

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
        var scopeList = ObjectSerializer.Deserialize<Scope>(filename + ".pirate");

        List<BaseValue> result = new();
        var interpreterFactory = new InterpreterFactory();
        foreach (var item in scopeList.Nodes)
        {
            Logger.Log($"Interpreting {item.GetType().Name}", this.GetType().Name, LogType.INFO);
            var interpreter = interpreterFactory.GetInterpreter(item, Logger);
            result.Add(interpreter.VisitNode());
        }
        return result;
    }
}