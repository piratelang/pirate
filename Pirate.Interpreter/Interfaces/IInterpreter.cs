using Pirate.Interpreter.Values;
using Pirate.Parser;

namespace Pirate.Interpreter.Interfaces;

/// <summary>
/// This interface contains all methods for the interpreter.
/// </summary>
public interface IInterpreter
{
    ILogger Logger { get; set; }

    List<BaseValue> StartInterpreter(Scope scope);
}
