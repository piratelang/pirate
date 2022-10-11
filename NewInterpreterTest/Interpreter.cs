using NewInterpreterTest.Interpreters;
using NewInterpreterTest.Values;
using NewParserTest;

namespace NewInterpreterTest;

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