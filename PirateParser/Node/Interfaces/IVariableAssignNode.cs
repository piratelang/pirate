using PirateParser.Node.Interfaces;

namespace PirateParser.Node.Interfaces;

public interface IVariableAssignNode : INode
{
    Token TypeToken { get; set; }
    IValueNode Identifier { get; set; }
    INode Value { get; set; }

    bool IsValid();
    string ToString();
}
