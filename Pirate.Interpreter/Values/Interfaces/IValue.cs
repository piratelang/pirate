namespace Pirate.Interpreter.Values.Interfaces;

public interface IValue : IValueTableItem
{
    object? Value { get; }
}