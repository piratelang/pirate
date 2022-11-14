using PirateParser.Node.Interfaces;

namespace PirateParser.Node.Interfaces;

public interface IVariableDeclarationNode : INode
{
    Token TypeToken { get; set; }
    IValueNode Identifier { get; set; }
    INode Value { get; set; }

    bool IsValid();
    string ToString();
}
