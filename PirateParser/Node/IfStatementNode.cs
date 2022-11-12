using PirateParser.Node.Interfaces;

namespace PirateParser.Node;

[Serializable]
public class IfStatementNode : IIfStatementNode
{
    public IOperationNode ConditionNode { get; set; }
    public List<INode> BodyNodes { get; set; }
    public List<INode> ElseNode { get; set; }

    public IfStatementNode(IOperationNode conditionNode, List<INode> bodyNodes, List<INode> elseNode)
    {
        ConditionNode = conditionNode;
        BodyNodes = bodyNodes;
        ElseNode = elseNode;
    }

    public IfStatementNode(IOperationNode conditionNode, List<INode> bodyNodes)
    {
        ConditionNode = conditionNode;
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
        return $"if ({ConditionNode.ToString()}) \n{{ \n {string.Join(" ", BodyNodes)} \n}}";
    }
}