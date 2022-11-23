using PirateInterpreter.Values;
using PirateInterpreter.StandardLibrary.Interfaces;

namespace PirateInterpreter.StandardLibrary;

public class StandardLibraryFactory : IStandardLibraryFactory
{
    public BaseValue CallFunction(string libraryname, string functionName, List<BaseValue> parameters, ILogger logger)
    {
        switch (libraryname)
        {
            case "IO":
                return CallIOFunction(functionName, parameters, logger);
            default:
                logger.Log($"No library was found for name: {libraryname}", Common.Enum.LogType.ERROR);
                throw new NullReferenceException("Requested element from the Standard Library does not exist.");
        }
    }

    private BaseValue CallIOFunction(string functionName, List<BaseValue> parameters, ILogger logger)
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
