using Pirate.Common.Logger.Interfaces;
using Pirate.Interpreter.Values;
using Pirate.Interpreter.Values.Function;

namespace Pirate.Interpreter.StandardLibrary.Standard.Terminal;

public class ReadFunction : CSharpFunction
{
    public ReadFunction(ILogger logger) : base(null, logger) { }

    public override string Name => "Standard.Terminal.Read";
    public override string Description => "Reads a line from the terminal";
    public override string Parameters => "The line to print";

    public override List<BaseValue> Execute(List<object> arguments)
    {
        Logger.Info($"[Standard.Terminal.Read] called with {arguments.Count} parameters");

        if (arguments.Count > 0)
            arguments.ForEach(a => Console.Write(a?.ToString()));

        var result = Console.ReadLine();
        return new List<BaseValue> { new StringValue(result, Logger) };
    }
}
