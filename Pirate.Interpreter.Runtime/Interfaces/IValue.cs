using Pirate.Interpreter.Runtime;

namespace Pirate.Interpreter.Runtime.Interfaces;

public interface IValue
{
    object? Value { get; }
}