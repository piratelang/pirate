using PirateParser.Node;
using PirateParser.Node.Interfaces;

namespace PirateParser.Node;

public class FunctionDeclarationNode : INode
{
    public IValueNode identifier { get; set; }
    public List<INode> parameters { get; set; }
    public INode returnType { get; set; }
    public List<INode> statements { get; set; }

    public FunctionDeclarationNode(ValueNode Identifier, List<INode> Parameters, INode ReturnType, List<INode> Statements)
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