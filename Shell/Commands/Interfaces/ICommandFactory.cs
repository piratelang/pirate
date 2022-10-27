using Common.Interfaces;

namespace Shell.Commands.Interfaces;

public interface ICommandFactory
    {
        IInitCommand InitCommand { get; set; }
        INewCommand NewCommand { get; set; }
        IRunCommand RunCommand { get; set; }
        IBuildCommand BuildCommand { get; set; }
        ILogger Logger { get; set; }

        ICommand GetCommand(string commandArgument);
    }