using Pirate.Common.Logger.Interfaces;
using Pirate.Interpreter.Values;
using Pirate.Interpreter.Values.Function;

namespace Pirate.Interpreter.StandardLibrary.Standard.Terminal;

public class CharCodeAtFunction : CSharpFunction
{
    public CharCodeAtFunction(ILogger logger) : base(null, logger) { }

    public override string Name => "Standard.String.CharCodeAt";
    public override string Description => "Returns the Unicode value of the character at the specified index in the given string";
    public override string Parameters => "String, Index";

    public override List<BaseValue> Execute(List<object> arguments)
    {
        Logger.Info($"[{Name}] called with {arguments.Count} parameters");

        var str = arguments[0] is BaseValue value 
            ? value.Value?.ToString() 
            : arguments[0].ToString();
        var idx = arguments[1] is BaseValue value2 
            ? value2.Value is int intValue
                ? intValue
                : throw new InvalidOperationException()
            : (int)arguments[1];

        if (idx >= 0 && idx < str.Length)
        {
            var charValue = (int)str[idx];
            return new List<BaseValue> { new IntegerValue(charValue, Logger) };
        }
        else
        {
            return new List<BaseValue> { new IntegerValue(-1, Logger) };
        }
    }
}
