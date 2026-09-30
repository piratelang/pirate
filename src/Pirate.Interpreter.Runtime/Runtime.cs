using Pirate.Common.Logger.Interfaces;
using Pirate.Interpreter.Interfaces;
using Pirate.Interpreter.Values.Interfaces;
using System;
//using Pirate.Interpreter.StandardLibrary.Interfaces;

namespace Pirate.Interpreter.Runtime;

/// <summary>
/// A collection of variables values and functions.
/// </summary>
public class Runtime : IRuntime
{
    public IValueTable<IValue> Variables { get; set; }
    public IValueTable<IFunctionValue> Functions { get; set; }

    private ILogger _logger { get; set; }

    public Runtime(ILogger logger)
    {
        _logger = logger;
        Variables = new ValueTable<IValue>(_logger);
        Functions = new ValueTable<IFunctionValue>(_logger);
    }


}