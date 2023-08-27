using Pirate.Lexer.Enums;
using Pirate.Lexer.Tokens;
using Pirate.Parser.Node.Interfaces;

namespace Pirate.Parser.Node;

/// <summary>
/// A node representing a comparison operation.
/// </summary>
/// <example>
/// 1 == 2
/// </example>
public class ComparisonOperationNode : IOperationNode
{
    public INode Left { get; set; }
    public Token Operator { get; set; }
    public INode Right { get; set; }

    public ComparisonOperationNode(INode left, Token operatorToken, INode right)
    {
        Left = left;
        Operator = operatorToken;
        Right = right;
    }

    public override string ToString()
    {
        return $"{Left.ToString()} | {Operator.ToString()} | {Right.ToString()}";
    }

    public bool IsValid()
    {
        if (Left is not INode)
        {
            return false;
        }
        if (Operator.TokenType is not TokenType.DOUBLEEQUALS and not TokenType.NOTEQUALS and not TokenType.GREATERTHAN and not TokenType.GREATERTHANEQUALS and not TokenType.LESSTHAN and not TokenType.LESSTHANEQUALS)
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