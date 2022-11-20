using PirateParser.Node.Interfaces;

namespace PirateParser.Node.Interfaces;

public interface IFunctionCallNode : INode
{
    IValueNode Identifier { get; set; }
    List<INode> Parameters { get; set; }

    bool IsValid();
}
