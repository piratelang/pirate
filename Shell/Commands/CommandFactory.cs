using Common;

namespace Shell.Commands
{
    public class CommandFactory
    {
        public static Command GetCommand(string commandArgument, string version, Logger logger)
        {
            switch (commandArgument)
            {
                case "init":
                    return new InitCommand(version, logger);
                case "new":
                    return new NewCommand(version, logger);
                case "run":
                    return new RunCommand(version, logger);
                case "build":
                    return new BuildCommand(version, logger);
                default:
                    return null;
            }
        }
    }
}