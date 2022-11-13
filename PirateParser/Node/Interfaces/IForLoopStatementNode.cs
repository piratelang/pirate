namespace PirateParser.Node.Interfaces;

public interface IForLoopStatementNode : INode
{
    VariableAssignNode VariableNode { get; set; }
    ValueNode ValueNode { get; set; }
    List<INode> BodyNodes { get; set; }

    bool IsValid();
    string ToString();
}
