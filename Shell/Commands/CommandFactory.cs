using Common;
using Shell.Commands.Interfaces;

namespace Shell.Commands
{
    public class CommandFactory : ICommandFactory
    {
        public IInitCommand InitCommand { get; set; }
        public INewCommand NewCommand { get; set; }
        public IRunCommand RunCommand { get; set; }
        public IBuildCommand BuildCommand { get; set; }
        public ILogger Logger { get; set; }

        public CommandFactory(IInitCommand initCommand, INewCommand newCommand, IRunCommand runCommand, IBuildCommand buildCommand, ILogger logger)
        {
            InitCommand = initCommand;
            NewCommand = newCommand;
            RunCommand = runCommand;
            BuildCommand = buildCommand;
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
            }
            throw new NotImplementedException($"{commandArgument} is not a found command.");
        }
    }
}