namespace PirateInterpreter;

public sealed class SymbolTable
{
    private static SymbolTable? symbolTable;
    public Dictionary<string, INode> SymbolList { get; set; } = new ();
    public ILogger Logger { get; set; }

    private SymbolTable(ILogger logger)
    {
        Logger = logger;
    }

    public static SymbolTable Instance(ILogger logger)
    {
        if (symbolTable == null)
        {
            symbolTable = new SymbolTable(logger);
        }
        return symbolTable;
    }

    public INode Get(string name)
    {
        var value = SymbolList.GetValueOrDefault(name);
        if (value == null) 
        {
            Logger.Log($"No value was found for name: {name}", this.GetType().Name, Common.Enum.LogType.ERROR);
            throw new NullReferenceException("Requested element from the Symbol Table does not exist.");
        }
        Logger.Log($"Fetched {name}: {value.ToString()} from SymbolTable", this.GetType().Name, Common.Enum.LogType.INFO);
        return value;
    }

    public bool Set(string name, INode value)
    {
        SymbolList[name] = value;
        Logger.Log($"Added {name}: {value.ToString()} to SymbolTable", this.GetType().Name, Common.Enum.LogType.INFO);
        return true;
    }

    public bool Remove(string name)
    {
        SymbolList.Remove(name);
        Logger.Log($"Removed {name} from SymbolTable", this.GetType().Name, Common.Enum.LogType.INFO);
        return true;
    }
}