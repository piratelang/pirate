using Pirate.Common.Enum;
using Pirate.Common.Interfaces;
using Pirate.Lexer.Tokens;
using Pirate.Parser.Node.Interfaces;

namespace Pirate.Interpreter.Values;

/// <summary>
/// A function value.
/// </summary>
public class FunctionValue : BaseValue
{
    public IFunctionDeclarationNode FunctionDeclarationNode { get; set; }

    public FunctionValue(IFunctionDeclarationNode functionDeclarationNode, ILogger logger) : base(null, logger)
    {
        FunctionDeclarationNode = functionDeclarationNode;
        Logger.Log($"Created {GetType().Name} : \"{FunctionDeclarationNode.ToString()}\"", LogType.INFO);
    }

    public override string ToString()
    {
        return FunctionDeclarationNode.ToString();
    }

    public override BaseValue OperatedBy(Token Operator, BaseValue Value)
    {
        throw new InvalidOperationException($"Cannot operate {GetType().Name} by {Operator}");
    }
}