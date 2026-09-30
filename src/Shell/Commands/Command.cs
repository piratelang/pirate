using Shell.Commands.Interfaces;

namespace Shell.Commands;

/// <summary>
/// A base class for all commands.
/// </summary>
public abstract class Command : ICommand
{
    protected string Version { get; set; }
    protected ILogger Logger { get; set; }
    public Command(ILogger logger, IEnvironmentVariables environmentVariables)
    {
        Logger = logger;
        Version = environmentVariables.GetVariable("version");
    }

    public abstract void Help();
    public abstract object Run(string[] arguments);

    public virtual void Error(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n{message}");
        Console.ForegroundColor = ConsoleColor.White;

        throw new RuntimeCommandException(message);
    }
}