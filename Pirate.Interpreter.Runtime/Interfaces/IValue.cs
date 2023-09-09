using Pirate.Interpreter.Runtime;

namespace Pirate.Interpreter.Runtime.Interfaces;

public interface IValue : IValueTableItem
{
    object? Value { get; }
}