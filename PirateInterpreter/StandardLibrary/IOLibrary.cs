using PirateInterpreter.Values;

namespace PirateInterpreter.StandardLibrary;

public class IOLibrary
{
    public BaseValue Print(IList<BaseValue> parameters, ILogger logger)
    {
        logger.Log($"Print called with {parameters.Count} parameters", this.GetType().Name, Common.Enum.LogType.INFO);
        foreach (var parameter in parameters)
        {
            System.Console.WriteLine(parameter.Value.ToString());
        }
        return null;
    }

    public BaseValue Read(IList<BaseValue> parameters, ILogger logger)
    {
        logger.Log($"Read called with {parameters.Count} parameters", this.GetType().Name, Common.Enum.LogType.INFO);
        return new StringValue(System.Console.ReadLine(), logger);
    }
}