using PirateParser.Node.Interfaces;

namespace PirateParser.Node;

[Serializable]
public class ValueNode : IValueNode
{
    public Token Value { get; set; }

    public ValueNode(Token value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return $"{Value.ToString()}";
    }

    public bool IsValid()
    {
        if (Value is not Token)
        {
            return false;
        }
        return true;
    }
}