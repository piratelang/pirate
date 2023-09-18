using Pirate.Common.Logger.Interfaces;
using Pirate.Interpreter.Values;
using Pirate.Interpreter.Values.Function;

namespace Pirate.Interpreter.StandardLibrary.Standard.Terminal;

public class CharAtFunction : CSharpFunction
{
    public CharAtFunction(ILogger logger) : base(null, logger) { }

    public override string Name => "Standard.String.CharAt";
    public override string Description => "Returns the character at the specified index in the given string";
    public override string Parameters => "String, Index";

    public override List<BaseValue> Execute(List<object> arguments)
    {
        Logger.Info($"[{Name}] called with {arguments.Count} parameters");

        var str = arguments[0] is BaseValue value 
            ? value.Value?.ToString() ?? throw new InvalidOperationException()
            : arguments[0].ToString() ?? throw new InvalidOperationException();
        var idx = arguments[1] is BaseValue value2 
            ? int.Parse(value2.Value?.ToString() ?? throw new InvalidOperationException())
            : int.Parse(arguments[1].ToString() ?? throw new InvalidOperationException());

        if (idx >= 0 && idx < str.Length)
        {
            return new List<BaseValue> { new CharValue(str[idx].ToString(), Logger) };
        }
        else
        {
            return new List<BaseValue> { new CharValue("", Logger) };
        }
    }
}
