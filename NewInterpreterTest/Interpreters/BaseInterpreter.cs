using NewInterpreterTest.Values;
using NewParserTest.Node.Interfaces;
using NewInterpreterTest.Interpreters.Interfaces;

namespace NewInterpreterTest.Interpreters;

public abstract class BaseInterpreter
{
    public abstract BaseValue VisitNode();
}