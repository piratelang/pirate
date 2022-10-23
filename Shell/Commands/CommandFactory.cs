using Common;

namespace Shell.Commands
{
    public class CommandFactory
    {
        public static Command GetCommand(string commandArgument, string version, ILogger logger, string location)
        {
            switch (commandArgument)
            {
                case "init":
                    return new InitCommand(version, logger);
                case "new":
                    return new NewCommand(version, logger);
                case "run":
                    return new RunCommand(version, logger, new ObjectSerializer(location, logger), location);
                case "build":
                    return new BuildCommand(version, logger, new ObjectSerializer(location, logger), location);
                default:
                    return null;
            }
        }
    }
}