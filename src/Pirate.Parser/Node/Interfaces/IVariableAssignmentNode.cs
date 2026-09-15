namespace Pirate.Parser.Node.Interfaces;

/// <inheritdoc cref="VariableAssignmentNode"/>
public interface IVariableAssignmentNode : INode
{
    IValueNode Identifier { get; set; }
    INode Value { get; set; }

    new bool IsValid();
    new string ToString();
}
