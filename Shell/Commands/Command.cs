using Common;

namespace Shell.Commands;

public abstract class Command
{
    public string version { get; set; }
    public Logger logger { get; set; }

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