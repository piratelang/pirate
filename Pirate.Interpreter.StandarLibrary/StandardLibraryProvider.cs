using Pirate.Common.Logger.Interfaces;
using Pirate.Interpreter.Interfaces;
using Pirate.Interpreter.StandardLibrary.Standard.Terminal;
using Pirate.Interpreter.Values.Function;

namespace Pirate.Interpreter.StandarLibrary;

public class StandardLibraryProvider : IStandardLibraryProvider
{
    public List<CSharpFunction> functions = new() { };

    private readonly ILogger Logger;

    private void RegisterFunction(CSharpFunction function)
    {
        Logger.Info($"Registering function {function.Name}");
        functions.Add(function);
    }

    public List<CSharpFunction> GetFunction(string name)
    {
        Logger.Info($"Getting function {name}");
        var function = functions.FindAll(f => f.Name.Contains(name)) ?? throw new InvalidOperationException($"Function {name} not found");
        return function;
    }

    public StandardLibraryProvider(ILogger logger)
    {
        Logger = logger;

        logger.Info("Creating StandardLibraryProvider");

        // Standard.Terminal
        RegisterFunction(new PrintFunction(logger));
        RegisterFunction(new ReadFunction(logger));
    }
}
