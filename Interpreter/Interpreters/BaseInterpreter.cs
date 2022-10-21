using Interpreter.Values;
using Parser.Node.Interfaces;
using Interpreter.Interpreters.Interfaces;
using Common;

namespace Interpreter.Interpreters;

public abstract class BaseInterpreter
{
    public abstract BaseValue VisitNode();
    protected ILogger Logger {get; set; }
    public BaseInterpreter(ILogger logger)
    {
        Logger = logger;
    }
}