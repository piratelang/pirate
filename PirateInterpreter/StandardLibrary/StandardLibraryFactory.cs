using PirateInterpreter.Values;

namespace PirateInterpreter.StandardLibrary;

public class StandardLibraryFactory
{
    public BaseValue GetFunction(string libraryname, string functionName, List<BaseValue> parameters, ILogger logger)
    {
        switch (libraryname)
        {
            case "IO":
                return GetIOFunction(functionName, parameters, logger);
            default:
                logger.Log($"No library was found for name: {libraryname}", this.GetType().Name, Common.Enum.LogType.ERROR);
                throw new NullReferenceException("Requested element from the Standard Library does not exist.");
        }
    }

    private BaseValue GetIOFunction(string functionName, List<BaseValue> parameters, ILogger logger)
    {
        switch (functionName)
        {
            case "print":
                return new IOLibrary().Print(parameters, logger);
            case "read":
                return new IOLibrary().Read(parameters, logger);
        }
        throw new ArgumentNullException("functionName", $"Factory cannot find function {functionName}");
    }
}
