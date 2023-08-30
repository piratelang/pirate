using Shell.Commands.Interfaces;

namespace Shell.Commands;

/// <summary>
/// Return the right command based on the command name.
/// </summary>
public class CommandFactory : ICommandFactory
{
    public IInitCommand InitCommand { get; set; }
    public INewCommand NewCommand { get; set; }
    public IRunCommand RunCommand { get; set; }
    public IBuildCommand BuildCommand { get; set; }
    public IShellCommand ShellCommand { get; set; }
    public ILogger Logger { get; set; }

    public CommandFactory(IInitCommand initCommand, INewCommand newCommand, IRunCommand runCommand, IBuildCommand buildCommand, ILogger logger, IShellCommand shellCommand)
    {
        InitCommand = initCommand;
        NewCommand = newCommand;
        RunCommand = runCommand;
        BuildCommand = buildCommand;
        ShellCommand = shellCommand;
        Logger = logger;
    }
    public ICommand GetCommand(string commandArgument)
    {
        switch (commandArgument)
        {
            case "init":
                return (ICommand)InitCommand;
            case "new":
                return (ICommand)NewCommand;
            case "run":
                return (ICommand)RunCommand;
            case "build":
                return (ICommand)BuildCommand;
            case "shell":
                return (ICommand)ShellCommand;
        }
        throw new NotImplementedException($"{commandArgument} is not a found command.");
    }
}
