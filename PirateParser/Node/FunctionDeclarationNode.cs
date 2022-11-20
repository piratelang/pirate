using PirateParser.Node.Interfaces;

namespace PirateParser.Node;

[Serializable]
public class FunctionDeclarationNode : IFunctionDeclarationNode
{
    public IValueNode Identifier { get; set; }
    public List<INode> Parameters { get; set; }
    public Token ReturnType { get; set; }
    public List<INode> Statements { get; set; }

    public FunctionDeclarationNode(ValueNode identifier, List<INode> parameters, Token returnType, List<INode> statements)
    {
        Identifier = identifier;
        Parameters = parameters;
        ReturnType = returnType;
        Statements = statements;
    }

    public bool IsValid()
    {
        if (Identifier is not IValueNode)
        {
            return false;
        }
        if (Parameters is not List<INode>)
        {
            return false;
        }
        if (ReturnType is not INode)
        {
            return false;
        }
        if (Statements is not List<INode>)
        {
            return false;
        }
        return true;
    }
}