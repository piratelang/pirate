using Lexer.Tokens;

namespace Parser.Node.Interfaces;

public interface IOperationNode : INode
{
    INode Left { get; set; }
    Token Operator { get; set; }
    INode Right { get; set; }
}

