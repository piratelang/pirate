using Shell.Commands;

namespace Shell;

/// <summary>
/// A class starting the application.
/// Firstly checks for arguments, otherwise starts the command manager.
/// </summary>
public class Application
{
    private readonly ICommandManager _commandManager;

    public Application(ICommandManager commandManager)
    {
        _commandManager = commandManager;
    }


    public int Run(string[] args, string version)
    {
        if (args.Length == 0)
        {
            NoCommand.Run(version);
            return 0;
        }
        else
        {
            return _commandManager.RunCommand(args);
        }
    }
}
