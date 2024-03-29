namespace Pirate.Parser.Node.Interfaces;

/// <summary>
/// Interface for all nodes.
/// </summary>
public interface INode
{
    string ToString();

    bool IsValid();
}
