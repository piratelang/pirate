using PirateParser.Node.Interfaces;

namespace PirateParser.Node;

/// <summary>
/// A node representing a for loop.
/// </summary>
/// <example>
/// for var i - 0 to 10
/// {
///    IO.print(i);
/// }
/// </example>
public class ForLoopStatementNode : IForLoopStatementNode
{
    public IVariableDeclarationNode VariableNode { get; set; }
    public IValueNode ValueNode { get; set; }
    public IList<INode> BodyNodes { get; set; }

    public ForLoopStatementNode(IVariableDeclarationNode variableNode,IValueNode valueNode, IList<INode> bodyNodes)
    {
        VariableNode = variableNode;
        ValueNode = valueNode;
        BodyNodes = bodyNodes;
    }

    public bool IsValid()
    {
        if (VariableNode is not IVariableDeclarationNode)
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