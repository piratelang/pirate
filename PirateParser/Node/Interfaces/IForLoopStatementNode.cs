namespace PirateParser.Node.Interfaces;

public interface IForLoopStatementNode : INode
{
    IVariableDeclarationNode VariableNode { get; set; }
    IValueNode ValueNode { get; set; }
    IList<INode> BodyNodes { get; set; }

    bool IsValid();
    string ToString();
}
