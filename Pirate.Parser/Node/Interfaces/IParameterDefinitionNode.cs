using Pirate.Lexer.Tokens;
using Pirate.Parser.Node;

namespace Pirate.Parser.Node.Interfaces;

/// <inheritdoc cref="ParameterDefinitionNode"/>
public interface IParameterDefinitionNode : INode
{
    Token TypeToken { get; set; }
    IValueNode Identifier { get; set; }
}
