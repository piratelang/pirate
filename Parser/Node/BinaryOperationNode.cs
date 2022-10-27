using Parser.Node.Interfaces;

namespace Parser.Node;

[Serializable]
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
        if (Operator.TokenType is not TokenOperators)
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
