using PirateInterpreter.Values;

namespace PirateInterpreter.Interfaces;

public interface IInterpreter
{
    ILogger Logger { get; set; }

    List<BaseValue> StartInterpreter(string filename);
}
