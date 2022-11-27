namespace PirateParser.Node.Interfaces;

/// <inheritdoc cref="FunctionCallNode"/>
public interface IFunctionCallNode : INode
{
    IValueNode Identifier { get; set; }
    List<INode> Parameters { get; set; }

    bool IsValid();
}

