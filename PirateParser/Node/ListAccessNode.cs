using PirateParser.Node.Interfaces;

namespace PirateParser.Node;

public interface IListAccessNode : INode
{
    IValueNode Identifier { get; set; }
    IValueNode Index { get; set; }
}

public class ListAccessNode : IListAccessNode
{
    public IValueNode Identifier { get; set; }
    public IValueNode Index { get; set; }

    public ListAccessNode(IValueNode identifier, IValueNode index)
    {
        Identifier = identifier;
        Index = index;
    }

    public bool IsValid()
    {
        return Identifier.IsValid() && Index.IsValid();
    }

    public override string ToString()
    {
        return $"{Identifier.ToString()}[{Index.ToString()}]";
    }
}
