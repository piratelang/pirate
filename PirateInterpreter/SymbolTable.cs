using Pirate.Common.Enum;
using Pirate.Common.Interfaces;
using Pirate.Interpreter.Values;

namespace Pirate.Interpreter;

/// <summary>
/// A collection of variables values and functions.
/// </summary>
public sealed class SymbolTable
{
    private static SymbolTable? symbolTable = default!;
    public Dictionary<string, BaseValue> SymbolList { get; set; } = new();
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
        if (value == null) throw new NullReferenceException("Requested element from the Symbol Table does not exist.");

        Logger.Log($"Fetched {name}: {value.ToString()} from SymbolTable", LogType.INFO);
        return value;
    }

    public bool SetBaseValue(string name, BaseValue value)
    {
        SymbolList[name] = value;
        Logger.Log($"Added {name}: {value.ToString()} to SymbolTable", LogType.INFO);
        Logger.Log($"SymbolTable now contains {SymbolList[name]}", LogType.INFO);
        return true;
    }

    public bool Remove(string name)
    {
        SymbolList.Remove(name);
        Logger.Log($"Removed {name} from SymbolTable", LogType.INFO);
        return true;
    }

    private void FillSymbolTable()
    {
        symbolTable?.SetBaseValue("IO", new Library(
            "IO",
            new List<string> { "read", "print" },
            Logger)
        );
    }
}