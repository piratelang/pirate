using PirateParser.Node.Interfaces;

namespace PirateParser.Node;

public class ParameterDefinitionNode : IParameterDefinitionNode
{
    public Token typeToken { get; set; }
    public IValueNode identifier { get; set; }

    public ParameterDefinitionNode(Token TypeToken, IValueNode Identifier)
    {
        typeToken = TypeToken;
        identifier = Identifier;
    }

    public bool IsValid()
    {
        if (typeToken is not Token)
        {
            return false;
        }
        if (identifier is not IValueNode)
        {
            return false;
        }
        return true;
    }
}