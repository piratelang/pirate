using PirateInterpreter.Values;
using PirateInterpreter.StandardLibrary.Interfaces;

namespace PirateInterpreter.StandardLibrary;

public class StandardLibraryCallManager : IStandardLibraryCallManager
{
    private readonly ILogger Logger;

    public StandardLibraryCallManager(ILogger logger)
    {
        Logger = logger;
    }

    public BaseValue CallFunction(string libraryname, string functionName, List<BaseValue> parameters)
    {
        switch (libraryname)
        {
            case "IO":
                return CallIOFunction(functionName, parameters);
            default:
                throw new NullReferenceException("Requested element from the Standard Library does not exist.");
        }
    }

    private BaseValue CallIOFunction(string functionName, List<BaseValue> parameters)
    {
        switch (functionName)
        {
            case "print":
                return new IOLibrary(Logger).Print(parameters);
            case "read":
                return new IOLibrary(Logger).Read(parameters);
        }
        throw new ArgumentNullException("functionName", $"Factory cannot find function {functionName}");
    }
}
