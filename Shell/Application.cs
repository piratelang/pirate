using Shell.Commands;

namespace Shell;

public class Application
{
    private readonly ICommandManager _commandManager;

    public Application(ICommandManager commandManager)
    {
        this._commandManager = commandManager;
    }


    public void Run(string[] args, string version, string location)
    {
        if (args.Length == 0)
        {
            Help(version);
        }
        else
        {
            _commandManager.RunCommand(args);
        }
    }

    public void Help(string version)
    {
        Console.WriteLine(String.Join(
            Environment.NewLine,
            $"\nPirateLang version {version}\n",
            "Commands:",
            " - pirate run [filename].pirate",
            "    run the specified file",
            " - pirate init [filename]",
            "    initializes a new pirate project",
            " - pirate new [type]",
            "    create a new file",
            " - pirate build",
            "    build the modules in the current folder"
        ));
    }
}
