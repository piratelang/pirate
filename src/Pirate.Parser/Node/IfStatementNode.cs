using Pirate.Parser.Node.Interfaces;

namespace Pirate.Parser.Node;

/// <summary>
/// A node representing a if statement.<br/>
/// If a else statement is present, the else statement is stored in the ElseStatement property.
/// </summary>
/// <example>
/// if 1 == 2
/// {<br/>
///    IO.print("Hello World");<br/>
/// }<br/>
/// else 
/// {<br/>
///   IO.print("Goodbye World");<br/>
/// }
/// </example>
public class IfStatementNode : IIfStatementNode
{
    public IOperationNode ConditionNode { get; set; }
    public List<INode> BodyNodes { get; set; }
    public List<INode> ElseNode { get; set; }

    public IfStatementNode(IOperationNode conditionNode, List<INode> bodyNodes, List<INode> elseNode = default!)
    {
        ConditionNode = conditionNode;
        BodyNodes = bodyNodes;
        ElseNode = elseNode;
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
        return $"if {ConditionNode.ToString()} \n{{ \n {string.Join(" ", BodyNodes)} \n}}";
    }
}