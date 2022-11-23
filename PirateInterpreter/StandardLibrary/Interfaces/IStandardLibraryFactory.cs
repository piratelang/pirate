using PirateInterpreter.Values;

namespace PirateInterpreter.StandardLibrary.Interfaces;

public interface IStandardLibraryFactory
{
    BaseValue CallFunction(string libraryname, string functionName, List<BaseValue> parameters, ILogger logger);
}
