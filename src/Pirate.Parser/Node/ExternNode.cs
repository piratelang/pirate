using Pirate.Parser.Node.Interfaces;

namespace Pirate.Parser.Node;

public class ExternNode : IExternNode
{
    public IValueNode FunctionIdentifier { get; set; }

    public ExternNode(IValueNode functionIdentifier)
    {
        FunctionIdentifier = functionIdentifier;
    }

    public bool IsValid()
    {
        return FunctionIdentifier.IsValid();
    }

    public override string ToString()
    {
        return $"extern {FunctionIdentifier}";
    }
}
