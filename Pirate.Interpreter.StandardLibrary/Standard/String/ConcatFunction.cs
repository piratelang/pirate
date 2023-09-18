using Pirate.Common.Logger.Interfaces;
using Pirate.Interpreter.Values;
using Pirate.Interpreter.Values.Function;

namespace Pirate.Interpreter.StandardLibrary.Standard.Terminal;

public class ConcatFunction : CSharpFunction
{
    public ConcatFunction(ILogger logger) : base(null, logger) { }

    public override string Name => "Standard.String.Concat";
    public override string Description => "Concatenates the given strings";
    public override string Parameters => "Multiple strings";

    public override List<BaseValue> Execute(List<object> arguments)
    {
        Logger.Info($"[{Name}] called with {arguments.Count} parameters");

        var result = "";
        foreach (var argument in arguments)
        {
            if (argument is BaseValue value)
            {
                result += value.Value?.ToString();
            }
            else
            {
                result += argument.ToString();
            }
        }

        return new List<BaseValue> { new StringValue(result, Logger) };
    }
}
