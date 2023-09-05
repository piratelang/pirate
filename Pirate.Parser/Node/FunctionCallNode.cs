using Pirate.Parser.Node.Interfaces;

namespace Pirate.Parser.Node;

/// <summary>
/// A node representing a function call.
/// </summary>
/// <example>
/// IO.print("Hello World");
/// </example>
public class FunctionCallNode : IFunctionCallNode
{
    public IValueNode Identifier { get; set; }
    public List<INode> Parameters { get; set; }

    public FunctionCallNode(IValueNode identifier, List<INode> parameters)
    {
        Identifier = identifier;
        Parameters = parameters;
    }

    public override string ToString()
    {
        return $"{Identifier.ToString()}({string.Join(", ", Parameters)})";
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
        return true;
    }
}