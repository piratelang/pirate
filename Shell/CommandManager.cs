using Shell.Commands.Interfaces;

namespace PirateLang;

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
        _logger.Info("Starting Command Factory");
        var command = _commandFactory.GetCommand(args[0]);
        if (command == null) { return; }

        if (args.Contains("-h") || args.Contains("--help"))
        {
            _logger.Info("Running Help Command");
            command.Help();
        }
        else
        {
            try
            {
                command.Run(args);
                _logger.Info("Command completed succesfully");
            }
            catch (Exception exception)
            {
                _logger.Error(exception);

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n{exception}");
                Console.ForegroundColor = ConsoleColor.White;
                throw;
            }
        }
    }
}
