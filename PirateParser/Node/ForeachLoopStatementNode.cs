using PirateParser.Node.Interfaces;

namespace PirateParser.Node;

public interface IForeachLoopStatementNode : INode
{
    ParameterDefinitionNode VariableAssign { get; }
    ValueNode Value { get; }
    List<INode> Nodes { get; }
}

public class ForeachLoopStatementNode : IForeachLoopStatementNode
{
    public ParameterDefinitionNode VariableAssign { get; set; }
    public ValueNode Value { get; set; }
    public List<INode> Nodes { get; set; }

    public ForeachLoopStatementNode(ParameterDefinitionNode variableAssign, ValueNode value, List<INode> nodes)
    {
        VariableAssign = variableAssign;
        Value = value;
        Nodes = nodes;
    }

    public bool IsValid()
    {
        return VariableAssign.IsValid() && Value.IsValid() && Nodes.All(n => n.IsValid());
    }
}