namespace Pirate.Parser.Node.Interfaces;

/// <inheritdoc cref="FunctionDeclarationNode"/>
public interface IFunctionDeclarationNode : INode
{
    IValueNode Identifier { get; set; }
    List<IParameterDefinitionNode> Parameters { get; set; }
    Token ReturnType { get; set; }
    List<INode> Statements { get; set; }
    INode ReturnNode { get; set; }

    new bool IsValid();
}
