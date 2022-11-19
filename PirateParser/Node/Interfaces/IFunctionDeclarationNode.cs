namespace PirateParser.Node.Interfaces;

public interface IFunctionDeclarationNode : INode
{
    IValueNode identifier { get; set; }
    List<INode> parameters { get; set; }
    Token returnType { get; set; }
    List<INode> statements { get; set; }

    bool IsValid();
}
