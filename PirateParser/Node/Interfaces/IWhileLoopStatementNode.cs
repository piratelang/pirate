namespace PirateParser.Node.Interfaces;

public interface IWhileLoopStatementNode : INode
{
    IOperationNode ConditionNode { get; set; }
    List<INode> BodyNodes { get; set; }

    bool IsValid();
    string ToString();
}
