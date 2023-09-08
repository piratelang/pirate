using Pirate.Common.Logger.Interfaces;
using Pirate.Interpreter.Runtime.Interfaces;

namespace Pirate.Interpreter.Runtime;

/// <summary>
/// A collection of variables values and functions.
/// </summary>
public class Runtime

{
    public Dictionary<string, IValue> VariableList { get; set; } = new();
    public Dictionary<string, IFunctionValue> FunctionList { get; set; } = new();

    private ILogger _logger { get; set; }

    public Runtime(ILogger logger)
    {
        _logger = logger;
    }


    public IValue GetVariable(string name)
    {
        var value = VariableList.GetValueOrDefault(name);
        if (value == null) throw new NullReferenceException("Requested element from the Runtime.VariableList does not exist.");

        _logger.Info($"Fetched {name}: {value.ToString()} from Runtime.VariableList");
        return value;
    }

    public bool SetVariable(string name, IValue value)
    {
        VariableList[name] = value;
        _logger.Debug($"Added {name}: {value.ToString()} to Runtime.VariableList");
        _logger.Info($"Runtime.VariableList now contains {VariableList[name]}");
        return true;
    }

    public bool RemoveVariable(string name)
    {
        VariableList.Remove(name);
        _logger.Info($"Removed {name} from Runtime.VariableList");
        return true;
    }
}