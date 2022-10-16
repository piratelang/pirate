using Lexer.Tokens;
using Parser.Node.Interfaces;
using Parser.Node;

namespace Parser.Node;

[Serializable]
public class BinaryOperationNode : INode, IOperationNode
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
}
