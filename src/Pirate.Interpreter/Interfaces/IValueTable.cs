using Pirate.Interpreter.Values.Interfaces;

namespace Pirate.Interpreter.Interfaces;

public interface IValueTable<T> where T : IValueTableItem
{
    T Get(string name);
    bool Set(string name, T value);
    bool Remove(string name);
}

