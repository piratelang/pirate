using PirateParser.Node.Interfaces;

namespace PirateParser.Node.Interfaces;

public interface IVariableAssignmentNode : INode
{
    IValueNode Identifier { get; set; }
    INode Value { get; set; }

    bool IsValid();
    string ToString();
}
