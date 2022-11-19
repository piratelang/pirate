using PirateParser.Node.Interfaces;

namespace PirateParser.Node;

public class FunctionDeclarationNode : IFunctionDeclarationNode
{
    public IValueNode identifier { get; set; }
    public List<INode> parameters { get; set; }
    public Token returnType { get; set; }
    public List<INode> statements { get; set; }

    public FunctionDeclarationNode(ValueNode Identifier, List<INode> Parameters, Token ReturnType, List<INode> Statements)
    {
        identifier = Identifier;
        parameters = Parameters;
        returnType = ReturnType;
        statements = Statements;
    }

    public bool IsValid()
    {
        if (identifier is not IValueNode)
        {
            return false;
        }
        if (parameters is not List<INode>)
        {
            return false;
        }
        if (returnType is not INode)
        {
            return false;
        }
        if (statements is not List<INode>)
        {
            return false;
        }
        return true;
    }
}