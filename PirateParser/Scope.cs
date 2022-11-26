using PirateParser.Node.Interfaces;

namespace PirateParser;

[Serializable]
public class Scope
{
    public List<INode> Nodes { get; private set; }
    private ILogger Logger { get; set; }

    public Scope(ILogger logger)
    {
        Logger = logger;
        Nodes = new();
    }

    public bool AddNode(INode node)
    {
        try
        {
            Nodes.Add(node);
            return true;
        }
        catch (Exception ex)
        {
            Logger.Log($"Scope.AddNode() returned an error of {ex.ToString()}", LogType.ERROR);
            return false;
        }

    }
}
