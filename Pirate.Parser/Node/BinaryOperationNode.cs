using Pirate.Lexer.Tokens;
using Pirate.Lexer.TokenType.Enums;
using Pirate.Parser.Node.Interfaces;

namespace Pirate.Parser.Node;

/// <summary>
/// A node representing a binary operation.
/// </summary>
/// <example>
/// 1 + 2
/// </example>
public class BinaryOperationNode : IOperationNode
{
    public INode Left { get; set; }
    public Token Operator { get; set; }
    public INode Right { get; set; }

    public BinaryOperationNode(INode left, Token operatorToken, INode right)
    {
        Left = left;
        Operator = operatorToken;
        Right = right;
    }

    public override string ToString()
    {
        return $"({Left.ToString()} | {Operator.ToString()} | {Right.ToString()})";
    }

    public bool IsValid()
    {
        if (Left is not INode)
        {
            return false;
        }
        if (Operator.TokenType is not TokenType.PLUS and not TokenType.MINUS and not TokenType.MULTIPLY and not TokenType.DIVIDE)
        {
            return false;
        }
        if (Right is not INode)
        {
            return false;
        }
        return true;
    }
}
