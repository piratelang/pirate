using PirateParser.Node.Interfaces;

namespace PirateParser.Node;

[Serializable]
public class ForLoopStatementNode : IForLoopStatementNode
{
    public IVariableAssignNode VariableNode { get; set; }
    public IValueNode ValueNode { get; set; }
    public IList<INode> BodyNodes { get; set; }

    public ForLoopStatementNode(IVariableAssignNode variableNode,IValueNode valueNode, IList<INode> bodyNodes)
    {
        VariableNode = variableNode;
        ValueNode = valueNode;
        BodyNodes = bodyNodes;
    }

    public bool IsValid()
    {
        if (VariableNode is not IVariableAssignNode)
        {
            return false;
        }
        if (ValueNode is not IValueNode)
        {
            return false;
        }
        if (BodyNodes is not IList<INode>)
        {
            return false;
        }
        return true;
    }

    public override string ToString()
    {
        string resultString = string.Empty;

        foreach (var node in BodyNodes)
        {
            resultString += node.ToString() + '\n';
        }
        return $"for {VariableNode.ToString()} to {ValueNode.ToString()}\n {{ \n {string.Join(" ", BodyNodes)} \n}}";
    }
}