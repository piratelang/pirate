using PirateInterpreter.Values;

namespace PirateInterpreter.StandardLibrary;

public class StandardLibraryFactory
{
    public BaseValue GetFunction(string functionName, List<BaseValue> parameters)
    {
        switch (functionName)
        {
            case "print":
                return new IOLibrary().Print(parameters);
        }
        throw new ArgumentNullException("functionName", $"Factory cannot find function {functionName}");
    }
}
