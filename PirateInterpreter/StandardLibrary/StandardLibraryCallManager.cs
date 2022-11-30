using PirateInterpreter.Values;
using PirateInterpreter.StandardLibrary.Interfaces;

namespace PirateInterpreter.StandardLibrary;

/// <summary>
/// Calls the right function from the standard library.
/// </summary>
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
            case "List":
                return CallListFunction(functionName, parameters);
            default:
                throw new NullReferenceException("Requested element from the Standard Library does not exist.");
        }
    }

    private BaseValue CallIOFunction(string functionName, List<BaseValue> parameters)
    {
        switch (functionName.ToLower())
        {
            case "print":
                return new IOLibrary(Logger).Print(parameters);
            case "read":
                return new IOLibrary(Logger).Read(parameters);
        }
        throw new ArgumentNullException("functionName", $"Factory cannot find function {functionName}");
    }

    private BaseValue CallListFunction(string functionName, List<BaseValue> parameters)
    {
        switch (functionName.ToLower())
        {
            case "add":
                return new ListLibrary(Logger).Add(parameters);
            case "remove":
                return new ListLibrary(Logger).Remove(parameters);
            case "get":
                return new ListLibrary(Logger).Get(parameters);
            case "set":
                return new ListLibrary(Logger).Set(parameters);
            case "contains":
                return new ListLibrary(Logger).Contains(parameters);
            case "size":
                return new ListLibrary(Logger).Size(parameters);
            case "clear":
                return new ListLibrary(Logger).Clear(parameters);
            case "zip":
                return new ListLibrary(Logger).Zip(parameters);
        }
        throw new ArgumentNullException("functionName", $"Factory cannot find function {functionName}");
    }
}
