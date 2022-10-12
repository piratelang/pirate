using Interpreter.Values;
using Parser.Node.Interfaces;
using Interpreter.Interpreters.Interfaces;

namespace Interpreter.Interpreters;

public abstract class BaseInterpreter
{
    public abstract BaseValue VisitNode();
}