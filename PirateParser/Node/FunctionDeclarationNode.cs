using PirateParser.Node.Interfaces;

namespace PirateParser.Node;

[Serializable]
public class FunctionDeclarationNode : IFunctionDeclarationNode
{
    public IValueNode Identifier { get; set; }
    public List<IParameterDefinitionNode> Parameters { get; set; }
    public Token ReturnType { get; set; }
    public List<INode> Statements { get; set; }
    public INode ReturnNode { get; set; } = default!;

    public FunctionDeclarationNode(ValueNode identifier, List<IParameterDefinitionNode> parameters, Token returnType, List<INode> statements)
    {
        Identifier = identifier;
        Parameters = parameters;
        ReturnType = returnType;
        Statements = statements;
    }

    public FunctionDeclarationNode(ValueNode identifier, List<IParameterDefinitionNode> parameters, Token returnType, List<INode> statements, INode returnNode)
    {
        Identifier = identifier;
        Parameters = parameters;
        ReturnType = returnType;
        Statements = statements;
        ReturnNode = returnNode;
    }
    
    public override string ToString()
    {
        var resultString = string.Empty;
        foreach (var node in Statements)
        {
            resultString += node.ToString() + '\n';
        }
        return $"function {Identifier.ToString()}({string.Join(", ", Parameters)}) : {ReturnType.ToString()}\n {{ \n {resultString} \n}}";
    }


    public bool IsValid()
    {
        if (Identifier is not IValueNode)
        {
            return false;
        }
        if (Parameters is not List<IParameterDefinitionNode>)
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
        if (ReturnNode is not INode || ReturnNode is null)
        {
            return false;
        }
        return true;
    }
}