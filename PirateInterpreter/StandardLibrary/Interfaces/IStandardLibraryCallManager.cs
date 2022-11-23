using PirateInterpreter.Values;

namespace PirateInterpreter.StandardLibrary.Interfaces;

public interface IStandardLibraryCallManager
{
    BaseValue CallFunction(string libraryname, string functionName, List<BaseValue> parameters);
}
