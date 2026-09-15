namespace Pirate.Parser.Node.Interfaces;

public interface IExternNode : INode
{
    public IValueNode FunctionIdentifier { get; set; }
}