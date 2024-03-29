namespace Pirate.Parser.Node.Interfaces;

/// <inheritdoc cref="ValueNode"/>
public interface IValueNode : INode
{
    Token Value { get; set; }
}
