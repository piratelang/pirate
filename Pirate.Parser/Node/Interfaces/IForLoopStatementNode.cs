namespace Pirate.Parser.Node.Interfaces;

/// <inheritdoc cref="ForLoopStatementNode"/>
public interface IForLoopStatementNode : INode
{
    IVariableDeclarationNode VariableNode { get; set; }
    IValueNode ValueNode { get; set; }
    IList<INode> BodyNodes { get; set; }

    new bool IsValid();
    new string ToString();
}
