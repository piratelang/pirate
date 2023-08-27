using Pirate.Lexer.Enums;
using Pirate.Lexer.Tokens;
using Pirate.Parser.Node.Interfaces;

namespace Pirate.Parser.Node;

/// <summary>
/// A node declaring a variable.
/// </summary>
/// <example>
/// var x = 5;
/// </example>
public class VariableDeclarationNode : IVariableDeclarationNode
{
    public Token TypeToken { get; set; }
    public IValueNode Identifier { get; set; }
    public INode Value { get; set; }

    public VariableDeclarationNode(Token typeToken, IValueNode identifier, INode value)
    {
        TypeToken = typeToken;
        Identifier = identifier;
        Value = value;
    }

    public override string ToString()
    {
        return $"({Identifier.ToString()} = {Value.ToString()})";
    }

    public bool IsValid()
    {
        if (TypeToken.TokenGroup is not TokenGroup.TYPEKEYWORD)
        {
            return false;
        }
        if (Identifier is not IValueNode)
        {
            return false;
        }
        if (Value is not INode)
        {
            return false;
        }
        return true;
    }
}