namespace Pirate.Interpreter.Runtime.Interfaces;

public interface IFunctionValue : IValueTableItem
{
    object Execute(List<object> arguments);
}
