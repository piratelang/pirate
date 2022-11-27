using Shell.Commands.Interfaces;

namespace Shell.Commands;

/// <summary>
/// A class which handles the commands.
/// </summary>
public class CommandManager : ICommandManager
{
    private readonly ILogger _logger;
    private readonly ICommandFactory _commandFactory;

    public CommandManager(ILogger logger, ICommandFactory commandFactory)
    {
        _logger = logger;
        _commandFactory = commandFactory;
    }

    public void RunCommand(string[] args)
    {
        _logger.Log("Starting Command Factory", LogType.INFO);
        var command = _commandFactory.GetCommand(args[0]);
        if (command == null) { return; }

        if (args.Contains("-h") || args.Contains("--help"))
        {
            _logger.Log("Running Help Command",  LogType.INFO);
            command.Help();
        }
        else
        {
            try
            {
                command.Run(args);
                _logger.Log("Command completed succesfully",  LogType.INFO);
            }
            catch (Exception exception)
            {
                _logger.Log(exception.ToString(),  LogType.ERROR);

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n{exception}");
                Console.ForegroundColor = ConsoleColor.White;
                throw exception;
            }
        }
    }
}
