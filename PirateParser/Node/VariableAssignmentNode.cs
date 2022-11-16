using PirateParser.Node.Interfaces;

namespace PirateParser.Node;

[Serializable]
public class VariableAssignmentNode : IVariableAssignmentNode
{
    public IValueNode Identifier { get; set; }
    public INode Value { get; set; }

    public VariableAssignmentNode(IValueNode identifier, INode value)
    {
        Identifier = identifier;
        Value = value;
    }

    public override string ToString()
    {
        return $"({Identifier.ToString()} = {Value.ToString()})";
    }

    public bool IsValid()
    {
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