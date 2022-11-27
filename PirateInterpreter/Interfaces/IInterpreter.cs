using PirateInterpreter.Values;

namespace PirateInterpreter.Interfaces;

/// <summary>
/// This interface contains all methods for the interpreter.
/// </summary>
public interface IInterpreter
{
    ILogger Logger { get; set; }

    List<BaseValue> StartInterpreter(string filename);
}
