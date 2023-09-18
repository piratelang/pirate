using Pirate.Common.Logger.Interfaces;
using Pirate.Interpreter.Values;
using Pirate.Interpreter.Values.Function;

namespace Pirate.Interpreter.StandardLibrary.Standard.Terminal;

public class LastIndexOfFunction : CSharpFunction
{
    public LastIndexOfFunction(ILogger logger) : base(null, logger) { }

    public override string Name => "Standard.String.LastIndexOf";
    public override string Description => "Returns the index of the last occurrence of the specified character in the given string";
    public override string Parameters => "String, Character";

    public override List<BaseValue> Execute(List<object> arguments)
    {
        Logger.Info($"[{Name}] called with {arguments.Count} parameters");

        var str = arguments[0] is BaseValue value 
            ? value.Value?.ToString() ?? throw new InvalidOperationException()
            : arguments[0].ToString() ?? throw new InvalidOperationException();
        var c = arguments[1] is BaseValue value2 
            ? value2.Value?.ToString() ?? throw new InvalidOperationException()
            : arguments[1].ToString() ?? throw new InvalidOperationException();

        for (int i = str.Length - 1; i >= 0; i--)
        {
            if (str[i].ToString() == c)
            {
                return new List<BaseValue> { new IntegerValue(i, Logger) };
            }
        }

        return new List<BaseValue> { new IntegerValue(-1, Logger) };
    }
}
