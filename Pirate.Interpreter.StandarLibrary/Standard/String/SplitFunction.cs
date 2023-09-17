using Pirate.Common.Logger.Interfaces;
using Pirate.Interpreter.Values;
using Pirate.Interpreter.Values.Function;

namespace Pirate.Interpreter.StandardLibrary.Standard.Terminal;

public class SplitFunction : CSharpFunction
{
    public SplitFunction(ILogger logger) : base(null, logger) { }

    public override string Name => "Standard.String.Split";
    public override string Description => "Splits the given string into a list of characters";
    public override string Parameters => "String";

    public override List<BaseValue> Execute(List<object> arguments)
    {
        throw new NotImplementedException();
        //Logger.Info($"[{Name}] called with {arguments.Count} parameters");

        //var str = arguments[0].ToString();
        //var list = new List<BaseValue>();

        //foreach (var c in str)
        //{
        //    list.Add(new StringValue(c.ToString(), Logger));
        //}

        //return list;
    }
}
