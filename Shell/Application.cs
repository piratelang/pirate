using Common;
using Common.Enum;
using Common.Interfaces;
using Shell.Commands;

namespace Shell;

public class Application
{
    public ILogger Logger { get; set; }
    public CommandFactory CommandFactory { get; set; }
    public Application(ILogger logger, CommandFactory commandFactory)
    {
        Logger = logger;
        CommandFactory = commandFactory;
    }


    public void Run(string[] args, string version, string location)
    {
        List<string> argumentsList = new();

        foreach (var item in args)
        {
            if (item != null)
            {
                argumentsList.Add(item);
            }
        }

        if (args.Length == 0)
        {
            Help(version);
        }
        else
        {
            RunCommand(args, argumentsList, version, location);
        }
    }

    public void Help(string version)
    {
        Console.WriteLine(String.Join(
            Environment.NewLine,
            $"\nPirateLang version {version}\n",
            "Commands:",
            " - pirate run [filename].pirate",
            "    run the specified file",
            " - pirate init [filename]",
            "    initializes a new pirate project",
            " - pirate new [type]",
            "    create a new file",
            " - pirate build",
            "    build the modules in the current folder"
        ));
    }

    public void RunCommand(string[] args, List<string> argumentsList, string version, string location)
    {
        Logger.Log("Starting Command Factory", "Program", LogType.INFO);
        var command = CommandFactory.GetCommand(args[0]);
        if (command == null) { return; }

        if (argumentsList.Contains("-h") || argumentsList.Contains("--help"))
        {
            Logger.Log("Running Help Command", "Program", LogType.INFO);
            command.Help();
        }
        else
        {
            try
            {
                var arguments = argumentsList.ToArray();
                command.Run(arguments);
                Logger.Log("Command completed succesfully", "Program", LogType.INFO);
            }
            catch (Exception exception)
            {
                Logger.Log(exception.ToString(), "Program", LogType.ERROR);

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n{exception}");
                Console.ForegroundColor = ConsoleColor.White;
                throw;
            }
        }
    }
}
