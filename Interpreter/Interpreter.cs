using System.Runtime.Serialization;
using Interpreter.Interpreters;
using Interpreter.Values;
using Parser;
using Common;
using Common.Enum;

namespace Interpreter;

public class Interpreter
{
    private Scope ScopeList { get; set; }
    public Logger Logger { get; set; }
    public Interpreter(string filename, ObjectSerializer objectSerializer, Logger logger)
    {
        ScopeList = objectSerializer.Deserialize<Scope>(filename + ".pirate");
        Logger = logger;
    }

    public List<BaseValue> StartInterpreter()
    {
        List<BaseValue> result = new();
        var interpreterFactory = new InterpreterFactory();
        foreach (var item in ScopeList.Nodes)
        {
            Logger.Log($"Interpreting {item.GetType().Name}", this.GetType().Name, LogType.INFO);
            var interpreter = interpreterFactory.GetInterpreter(item, Logger);
            result.Add(interpreter.VisitNode());
        }
        return result;
    }
}