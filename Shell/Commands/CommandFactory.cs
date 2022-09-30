namespace Shell.Commands
{
    public class CommandFactory
    {
        public static ICommand GetCommand(string commandArgument, string version)
        {
            switch (commandArgument)
            {
                case "init":
                    return new InitCommand(version);
                case "new":
                    return new NewCommand(version);
                case "run":
                    return new RunCommand(version);
                default:
                    return null;
            }
        }
    }
}