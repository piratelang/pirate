namespace Parser.Node.Interfaces;

public interface IValueNode : INode
{
    Token Value { get; set; }
}
