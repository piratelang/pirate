using Common;

namespace Shell.Commands;

public abstract class Command
{
    protected string Version { get; set; }
    protected ILogger Logger { get; set; }
    public Command(string version, ILogger logger)
    {
        Version = version;
        Logger = logger;
    }

    public abstract void Help();
    public virtual void Run(string[] arguments)
    {

    }
    public void Error(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n{message}");
        Console.ForegroundColor = ConsoleColor.White;
    }
}