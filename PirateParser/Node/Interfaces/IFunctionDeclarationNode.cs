using System.Reflection.Metadata.Ecma335;
namespace PirateParser.Node.Interfaces;

public interface IFunctionDeclarationNode : INode
{
    IValueNode Identifier { get; set; }
    List<IParameterDefinitionNode> Parameters { get; set; }
    Token ReturnType { get; set; }
    List<INode> Statements { get; set; }

    bool IsValid();
}
