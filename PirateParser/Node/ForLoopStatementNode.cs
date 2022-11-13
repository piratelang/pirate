using PirateParser.Node.Interfaces;

namespace PirateParser.Node;

[Serializable]
public class ForLoopStatementNode : IForLoopStatementNode
{
    public VariableAssignNode VariableNode { get; set; }
    public ValueNode ValueNode { get; set; }
    public List<INode> BodyNodes { get; set; }

    public ForLoopStatementNode(VariableAssignNode variableNode, ValueNode valueNode, List<INode> bodyNodes)
    {
        VariableNode = variableNode;
        ValueNode = valueNode;
        BodyNodes = bodyNodes;
    }

    public bool IsValid()
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        string resultString = string.Empty;

        foreach (var node in BodyNodes)
        {
            resultString += node.ToString() + '\n';
        }
        return $"if ({VariableNode.ToString()}) \n{{ \n {string.Join(" ", BodyNodes)} \n}}";
    }
}