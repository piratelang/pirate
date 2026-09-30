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


    public void Run(string[] args, string version)
    {
        if (args.Length == 0)
        {
            NoCommand.Run(version);
        }
        else
        {
            _commandManager.RunCommand(args);
        }
    }
}
