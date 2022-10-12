using Interpreter.Interpreters;
using Interpreter.Values;
using Parser;

namespace Interpreter;

public class Interpreter
{
    private Scope _scope { get; set; }
    public Interpreter(Scope scope)
    {
        _scope = scope;
    }

    public BaseValue StartInterpreter()
    {
        var interpreterFactory = new InterpreterFactory();
        foreach (var item in _scope.Nodes)
        {
            var interpreter = interpreterFactory.GetInterpreter(item);
            var result = interpreter.VisitNode();

            return result;
        }
        return null;
    }
}