using PirateParser.Node.Interfaces;

namespace PirateParser.Node.Interfaces;

public interface IListDeclarationNode : INode
{
    List<INode> Nodes { get; set; }
}
