namespace Pirate.Interpreter.Runtime.Interfaces
{
    public interface IRuntime
    {
        ValueTable<IFunctionValue> Functions { get; set; }
        ValueTable<IValue> Variables { get; set; }
    }
}