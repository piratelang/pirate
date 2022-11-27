namespace PirateParser.Node.Interfaces;

/// <inheritdoc cref="VariableAssignmentNode"/>
public interface IVariableAssignmentNode : INode
{
    IValueNode Identifier { get; set; }
    INode Value { get; set; }

    bool IsValid();
    string ToString();
}
