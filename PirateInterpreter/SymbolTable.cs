using PirateInterpreter.Values;

namespace PirateInterpreter;

public sealed class SymbolTable
{
    private static SymbolTable? symbolTable;
    public Dictionary<string, BaseValue> SymbolList { get; set; } = new ();
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
            symbolTable.FillSymbolTable();
        }
        return symbolTable;
    }

    public BaseValue GetBaseValue(string name)
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

    public bool SetBaseValue(string name, BaseValue value)
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

    private void FillSymbolTable()
    {
        // IO Functions, Require C# code
        symbolTable.SetBaseValue("print", null);
    }
}