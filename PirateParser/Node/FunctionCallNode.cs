using PirateParser.Node.Interfaces;

namespace PirateParser.Node;

[Serializable]
public class FunctionCallNode : IFunctionCallNode
{
    public IValueNode Identifier { get; set; }
    public List<INode> Parameters { get; set; }

    public FunctionCallNode(IValueNode identifier, List<INode> parameters)
    {
        Identifier = identifier;
        Parameters = parameters;
    }

    public bool IsValid()
    {
        if (Identifier is not IValueNode)
        {
            return false;
        }
        if (Parameters is not List<INode>)
        {
            return false;
        }
        return true;
    }
}