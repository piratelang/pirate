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
    public Interpreter(ObjectSerializer objectSerializer, Logger logger)
    {
        ScopeList = objectSerializer.Deserialize<Scope>("test.pirate");
        Logger = logger;
    }

    public BaseValue StartInterpreter()
    {
        var interpreterFactory = new InterpreterFactory();
        foreach (var item in ScopeList.Nodes)
        {
            Logger.Log($"Interpreting {item.GetType().Name}", this.GetType().Name, LogType.INFO);
            var interpreter = interpreterFactory.GetInterpreter(item, Logger);
            var result = interpreter.VisitNode();
            return result;
        }
        return null;
    }
}