using Parser.Node.Interfaces;
using Common;

namespace Interpreter;

public class SymbolTable
{
    public Dictionary<string, INode> SymbolList { get; set; }
    public Logger Logger { get; set; }
    public SymbolTable(Logger logger)
    {
        SymbolList = new();
        Logger = logger;
    }

    public INode Get(string name)
    {
        var value = SymbolList.GetValueOrDefault(name);
        if (value == null) Logger.Log($"No value was found for name: {name}", this.GetType().Name, Common.Enum.LogType.ERROR);
        else Logger.Log($"Fetched {name}: {value.ToString()} from SymbolTable", this.GetType().Name, Common.Enum.LogType.INFO);
        return value;
    }

    public void Set(string name, INode value)
    {
        SymbolList[name] = value;
        Logger.Log($"Added {name}: {value.ToString()} to SymbolTable", this.GetType().Name, Common.Enum.LogType.INFO);
    }

    public void Remove(string name)
    {
        SymbolList.Remove(name);
        Logger.Log($"Removed {name} from SymbolTable", this.GetType().Name, Common.Enum.LogType.INFO);
    }
}