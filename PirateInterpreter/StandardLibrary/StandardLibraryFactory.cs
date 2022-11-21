using PirateInterpreter.Values;

namespace PirateInterpreter.StandardLibrary;

public class StandardLibraryFactory
{
    public BaseValue GetFunction(string functionName, List<BaseValue> parameters, ILogger logger)
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
