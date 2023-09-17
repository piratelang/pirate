using Pirate.Common.Logger.Interfaces;
using Pirate.Interpreter.Interfaces;
using Pirate.Interpreter.Values.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Pirate.Interpreter.Runtime;

public class ValueTable<T> : IValueTable<T> where T : IValueTableItem
{
    private Dictionary<string, T> _values = new();
    private readonly ILogger _logger;

    public ValueTable(ILogger logger)
    {
        _logger = logger;
    }

    public T Get(string name)
    {
        var value = _values.GetValueOrDefault(name);
        if (value == null) throw new NullReferenceException($"Requested element: \"{name}\" does not exist in Runtime.ValueTable");

        _logger.Info($"Fetched {name}: {value.ToString()} from Runtime.ValueTable");
        return value;
    }

    public bool Set(string name, T value)
    {
        _values[name] = value;
        _logger.Debug($"Added {name}: {value.ToString()} to Runtime.ValueTable");
        _logger.Info($"Runtime.ValueTable now contains {_values[name]}");
        return true;
    }

    public bool Remove(string name)
    {
        _values.Remove(name);
        _logger.Info($"Removed {name} from Runtime.VariableList");
        return true;
    }


}

