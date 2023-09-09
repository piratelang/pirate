using Pirate.Common.Logger.Interfaces;
using Pirate.Interpreter.Runtime.Interfaces;

namespace Pirate.Interpreter.Runtime;

/// <summary>
/// A collection of variables values and functions.
/// </summary>
public class Runtime : IRuntime
{
    public ValueTable<IValue> Variables { get; set; }
    public ValueTable<IFunctionValue> Functions { get; set; }

    private ILogger _logger { get; set; }

    public Runtime(ILogger logger)
    {
        _logger = logger;
        Variables = new(_logger);
        Functions = new(_logger);
    }


}