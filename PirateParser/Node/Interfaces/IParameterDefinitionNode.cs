using PirateParser.Node.Interfaces;

namespace PirateParser.Node.Interfaces;

public interface IParameterDefinitionNode : INode
{
    Token TypeToken { get; set; }
    IValueNode Identifier { get; set; }
}
