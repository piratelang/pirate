using Pirate.Interpreter.Values.Interfaces;

namespace Pirate.Interpreter.Interfaces;

public interface IRuntime
{
    public IValueTable<IFunctionValue> Functions { get; set; }
    public IValueTable<IValue> Variables { get; set; }
}