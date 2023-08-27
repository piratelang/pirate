
using Pirate.Common.Interfaces;
using Pirate.Lexer.Tokens;

namespace Pirate.Interpreter.Values;

/// <summary>
/// A placeholder value for a standard library function.
/// </summary>
public class Library : BaseValue
{
    public string Name { get; set; }
    public List<string> Callables { get; set; }
    public Library(string name, List<string> callables, ILogger logger) : base(null, logger)
    {
        Name = name;
        Callables = callables;
        Logger.Log($"Created {GetType().Name}", LogType.INFO);
    }

    public override string ToString()
    {
        return Name;
    }

    public override BaseValue OperatedBy(Token Operator, BaseValue Value)
    {
        throw new InvalidOperationException($"Cannot operate {GetType().Name} by {Operator.ToString()}");
    }
}