using Parser.Node.Interfaces;
using Common;
using Parser.Node;
using Lexer.Tokens;
using Interpreter.Values;

namespace Interpreter;

public sealed class SymbolTable
{
    private static SymbolTable symbolTable = null;
    public Dictionary<string, INode> SymbolList { get; set; }
    public Logger Logger { get; set; }

    private SymbolTable(Logger logger)
    {
        SymbolList = new();
        Logger = logger;
    }

    public static SymbolTable Instance(Logger logger)
    {
        if (symbolTable == null)
        {
            symbolTable = new SymbolTable(logger);
            symbolTable.SymbolList["a"] = new ValueNode(new Token(Lexer.Enums.TokenGroup.VALUE, Lexer.Enums.TokenValue.INT, logger, 2));
        }
        return symbolTable;
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