using Pirate.Common.Logger.Interfaces;
using Pirate.Interpreter.Values;
using Pirate.Interpreter.Values.Function;

namespace Pirate.Interpreter.StandardLibrary.Standard.Terminal;

public class LengthFunction : CSharpFunction
{
    public LengthFunction(ILogger logger) : base(null, logger) { }

    public override string Name => "Standard.String.Length";
    public override string Description => "Gets the length of the given string";
    public override string Parameters => "string";

    public override List<BaseValue> Execute(List<object> arguments)
    {
        Logger.Info($"[{Name}] called with {arguments.Count} parameters");

        if (arguments.Count != 1) throw new InvalidOperationException($"Function {Name} expects 1 parameter");

        if (arguments[0] is BaseValue value)
        {
            return new List<BaseValue> { 
                new IntegerValue(value.Value?.ToString().Length ?? 0, Logger) 
            };
        }
        else
        {
            return new List<BaseValue> { new IntegerValue(arguments[0].ToString().Length, Logger) };
        }
    }
}