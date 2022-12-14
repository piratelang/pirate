using PirateInterpreter.Values;

namespace PirateInterpreter.StandardLibrary;

/// <summary>
/// Contains the standard library functions for the console.
/// </summary>
public class IOLibrary
{
    private readonly ILogger Logger;

    public IOLibrary(ILogger logger)
    {
        Logger = logger;
    }

    public BaseValue Print(IList<BaseValue> parameters)
    {
        Logger.Log($"Print called with {parameters.Count} parameters", LogType.INFO);
        foreach (var parameter in parameters)
        {
            System.Console.WriteLine(parameter.Value.ToString());
        }
        return null;
    }

    public StringValue Read(IList<BaseValue> parameters)
    {
        Logger.Log($"Read called with {parameters.Count} parameters", LogType.INFO);
        if (parameters[0] is not null) System.Console.Write(parameters[0].Value.ToString());
        return new StringValue(System.Console.ReadLine(), Logger);
    }
}