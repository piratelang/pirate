using PirateInterpreter.Values;

namespace PirateInterpreter.Interpreters;

public abstract class BaseInterpreter
{
    protected ILogger Logger {get; set; }
    protected InterpreterFactory InterpreterFactory { get; private set; }
    
    public BaseInterpreter(ILogger logger, InterpreterFactory interpreterFactory)
    {
        Logger = logger;
        InterpreterFactory = interpreterFactory;
    }
    
    public abstract BaseValue VisitNode();
}