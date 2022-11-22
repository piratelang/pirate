using PirateParser.Node.Interfaces;

namespace PirateParser.Node;

[Serializable]
public class ParameterDefinitionNode : IParameterDefinitionNode
{
    public Token TypeToken { get; set; }
    public IValueNode Identifier { get; set; }

    public ParameterDefinitionNode(Token typeToken, IValueNode identifier)
    {
        TypeToken = typeToken;
        Identifier = identifier;
    }

    public override string ToString()
    {
        return $"{TypeToken.ToString()} {Identifier.ToString()}";
    }

    public bool IsValid()
    {
        if (TypeToken is not Token)
        {
            return false;
        }
        if (Identifier is not IValueNode)
        {
            return false;
        }
        return true;
    }
}