namespace Pirate.Interpreter.Runtime.Interfaces;

public interface IFunctionValue
{
    object Execute(List<object> arguments);
}
