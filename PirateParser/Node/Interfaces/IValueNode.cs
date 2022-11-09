namespace PirateParser.Node.Interfaces;

public interface IValueNode : INode
{
    Token Value { get; set; }
}
