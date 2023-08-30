using Pirate.Interpreter.Values;

namespace Pirate.Interpreter.StandardLibrary;

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
        var result = "";
        foreach (var parameter in parameters)
        {
            Console.WriteLine(parameter.Value.ToString());
            result += parameter.Value.ToString();
        }
        return new StringValue(result, Logger);
    }

    public StringValue Read(IList<BaseValue> parameters)
    {
        Logger.Log($"Read called with {parameters.Count} parameters", LogType.INFO);
        if (parameters[0] is not null) Console.Write(parameters[0].Value.ToString());
        return new StringValue(Console.ReadLine(), Logger);
    }
}