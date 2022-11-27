using PirateParser.Node.Interfaces;

namespace PirateParser.Node;

/// <summary>
/// A node defining a while loop.
/// </summary>
/// <example>
/// while x < 5 
/// {
///    x = x + 1;
/// }
/// </example>    
public class WhileLoopStatementNode : IWhileLoopStatementNode
{
    public IOperationNode ConditionNode { get; set; }
    public List<INode> BodyNodes { get; set; }

    public WhileLoopStatementNode(IOperationNode conditionNode, List<INode> bodyNodes)
    {
        ConditionNode = conditionNode;
        BodyNodes = bodyNodes;
    }

    public bool IsValid()
    {
        if (ConditionNode is not IOperationNode)
        {
            return false;
        }
        if (BodyNodes is not List<INode>)
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
        return $"while ({ConditionNode.ToString()}) \n{{ \n {string.Join(" ", BodyNodes)} \n}}";
    }
}