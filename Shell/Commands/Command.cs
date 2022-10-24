using Common;
using Common.Errors;
using Shell.Commands.Interfaces;

namespace Shell.Commands;

public abstract class Command : ICommand
{
    protected string Version { get; set; } = EnvironmentVariables.GetVariable("version");
    protected ILogger Logger { get; set; }
    public Command(ILogger logger)
    {
        Logger = logger;
    }

    public abstract void Help();
    public abstract void Run(string[] arguments);
    public void Error(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n{message}");
        Console.ForegroundColor = ConsoleColor.White;

        throw new RuntimeCommandException(message);
    }
}