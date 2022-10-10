using NewParserTest;

namespace NewInterpreterTest;

public class Interpreter
{
    private Scope _scope { get; set; }
    public Interpreter(Scope scope)
    {
        _scope = scope;
    }
}