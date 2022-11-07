using PirateInterpreter.Values;

namespace PirateInterpreter;

public interface IInterpreter
{
    ILogger Logger { get; set; }

    List<BaseValue> StartInterpreter(string filename);
}
