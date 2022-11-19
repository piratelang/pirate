using PirateParser.Node.Interfaces;

namespace PirateParser.Node.Interfaces;

public interface IParameterDefinitionNode : INode
{
    Token typeToken { get; set; }
    IValueNode identifier { get; set; }
}
