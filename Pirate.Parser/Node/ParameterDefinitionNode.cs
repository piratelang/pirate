using Pirate.Parser.Node.Interfaces;

namespace Pirate.Parser.Node;

/// <summary>
/// A node defining a parameter.
/// It contains a type and a name.
/// </summary>
/// <example>
/// int x
/// </example>
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