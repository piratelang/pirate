namespace PirateParser.Node.Interfaces;

/// <inheritdoc cref="WhileLoopStatementNode"/>
public interface IWhileLoopStatementNode : INode
{
    IOperationNode ConditionNode { get; set; }
    List<INode> BodyNodes { get; set; }

    bool IsValid();
    string ToString();
}
