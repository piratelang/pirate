using Pirate.Interpreter.Interpreters;
using Pirate.Parser.Node.Interfaces;

namespace Pirate.Interpreter.Interpreters.Interfaces;

/// <inheritdoc cref="InterpreterFactory"/>
public interface IInterpreterFactory
{
    BaseInterpreter GetInterpreter(INode node);
}
