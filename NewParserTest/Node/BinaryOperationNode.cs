using NewPirateLexer.Tokens;
using NewParserTest.Node.Interfaces;
using NewParserTest.Node;

namespace NewParserTest.Node;

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

    public string Display()
    {
        return $"({Left.Display()} | {Operator.Display()} | {Right.Display()})";
    }
}
