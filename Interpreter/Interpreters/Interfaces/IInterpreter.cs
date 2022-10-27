using Parser.Node.Interfaces;

namespace Interpreter.Interpreters.Interfaces;

public interface IInterpreter
{
    INode Node { get; set; }
}
