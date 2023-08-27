using Pirate.Lexer.Tokens;

namespace Pirate.Parser.Node.Interfaces;


/// <summary>
/// Interface for all operation nodes.
/// </summary>
public interface IOperationNode : INode
{
    INode Left { get; set; }
    Token Operator { get; set; }
    INode Right { get; set; }
}

