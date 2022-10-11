using NewInterpreterTest.Values;
using NewParserTest.Node.Interfaces;

namespace NewInterpreterTest.Interpreters.Interfaces;

public interface IInterpreter
{
    INode Node { get; set; }
}
