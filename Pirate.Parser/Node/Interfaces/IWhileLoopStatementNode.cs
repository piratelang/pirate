namespace Pirate.Parser.Node.Interfaces;

/// <inheritdoc cref="WhileLoopStatementNode"/>
public interface IWhileLoopStatementNode : INode
{
    IOperationNode ConditionNode { get; set; }
    List<INode> BodyNodes { get; set; }

    new bool IsValid();
    new string ToString();
}
