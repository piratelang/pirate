using Shell.Commands.Interfaces;
using Shell.Commands;

namespace PirateLang;

/// <summary>
/// A class which handles the commands.
/// </summary>
public class CommandManager : ICommandManager
{
    private readonly ILogger _logger;
    private readonly ICommandFactory _commandFactory;
    private readonly string _version;

    public CommandManager(ILogger logger, ICommandFactory commandFactory, IEnvironmentVariables environmentVariables)
    {
        _logger = logger;
        _commandFactory = commandFactory;
        _version = environmentVariables.GetVariable("version");
    }

    public int RunCommand(string[] args)
    {
        if (args.Length == 0) { return 0; }

        if (args[0] == "-h" || args[0] == "--help")
        {
            NoCommand.Run(_version);
            return 0;
        }

        if (args[0] == "-v" || args[0] == "--version")
        {
            Console.WriteLine($"PirateLang version {_version}");
            return 0;
        }

        if (args[0].StartsWith("-"))
        {
            Console.WriteLine($"Unknown option \"{args[0]}\".");
            return 1;
        }

        _logger.Info("Starting Command Factory");
        try
        {
            var command = _commandFactory.GetCommand(args[0]);
            var commandArguments = args.Skip(1).ToArray();

            if (commandArguments.Any(argument => argument.StartsWith("-")))
            {
                if (
                    commandArguments.Length == 1 &&
                    (commandArguments[0] == "-h" || commandArguments[0] == "--help")
                )
                {
                    _logger.Info("Running Help Command");
                    command.Help();
                    return 0;
                }

                Console.WriteLine($"Unknown option \"{commandArguments.First(argument => argument.StartsWith("-"))}\".");
                return 1;
            }

            if (!TryValidateCommandArgumentCount(args[0], commandArguments.Length))
            {
                command.Help();
                return 1;
            }

            command.Run(args);
            _logger.Info("Command completed succesfully");
            return 0;
        }
        catch (Exception exception)
        {
            _logger.Error(exception);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n{exception.Message}");
            Console.ForegroundColor = ConsoleColor.White;
            return 1;
        }
    }

    private bool TryValidateCommandArgumentCount(string commandName, int commandArgumentCount)
    {
        return commandName switch
        {
            "build" or "shell" => commandArgumentCount == 0,
            "init" or "run" => commandArgumentCount <= 1,
            "new" => commandArgumentCount <= 2,
            _ => true
        };
    }
}
