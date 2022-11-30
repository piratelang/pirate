using PirateParser.Node.Interfaces;

namespace PirateParser.Node;

public class ListDeclarationNode : IListDeclarationNode
{
    public List<INode> Nodes { get; set; }

    public ListDeclarationNode(List<INode> nodes)
    {
        Nodes = nodes;
    }

    public bool IsValid()
    {
        if (Nodes is not List<INode>)
        {
            return false;
        }
        return true;
    }

    public override string ToString()
    {
        string resultString = string.Empty;

        foreach (var node in Nodes)
        {
            resultString += node.ToString() + '\n';
        }
        return $"[ {resultString} ]";
    }
}