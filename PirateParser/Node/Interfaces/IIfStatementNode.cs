namespace PirateParser.Node.Interfaces;

public interface IIfStatementNode : INode
{
    IOperationNode ConditionNode { get; set; }
    List<INode> BodyNodes { get; set; }
    List<INode> ElseNode { get; set; }
}
