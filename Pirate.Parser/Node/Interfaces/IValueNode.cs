using Pirate.Lexer.Tokens;
using Pirate.Parser.Node;

namespace Pirate.Parser.Node.Interfaces;

/// <inheritdoc cref="ValueNode"/>
public interface IValueNode : INode
{
    Token Value { get; set; }
}
