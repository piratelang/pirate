namespace Pirate.Interpreter.Interpreters.Interfaces;

/// <inheritdoc cref="InterpreterFactory"/>
public interface IInterpreterFactory
{
    BaseInterpreter GetInterpreter(INode node);
}
