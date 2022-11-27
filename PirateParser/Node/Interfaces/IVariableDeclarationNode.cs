namespace PirateParser.Node.Interfaces;

/// <inheritdoc cref="VariableDeclarationNode"/>
public interface IVariableDeclarationNode : INode
{
    Token TypeToken { get; set; }
    IValueNode Identifier { get; set; }
    INode Value { get; set; }

    bool IsValid();
    string ToString();
}
