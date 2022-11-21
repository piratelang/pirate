using PirateInterpreter.Values;

namespace PirateInterpreter.StandardLibrary;

public class IOLibrary
{
    public BaseValue Print(IList<BaseValue> parameters)
    {
        foreach (var parameter in parameters)
        {
            System.Console.WriteLine(parameter.Value.ToString());
        }
        return null;
    }
}