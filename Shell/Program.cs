using System.Globalization;
// See https://aka.ms/new-console-template for more information
using Shell.Commands;
using Common;
using Common.Enum;

namespace Shell;

internal class Program
{
    static void Main(string[] args)
    {

        var version = "1.0.0";
        var location = $"bin/pirate{version}";
        var logger = new Logger(version);

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
            Console.WriteLine($"\nPirateLang version {version}\n");
            Console.WriteLine("Commands:");
            Console.WriteLine(" - pirate run [filename].pirate");
            Console.WriteLine("    run the specified file");
            Console.WriteLine(" - pirate init [filename]");
            Console.WriteLine("    initializes a new pirate project");
            Console.WriteLine(" - pirate new [type]");
            Console.WriteLine("    create a new file");
            Console.WriteLine(" - pirate build");
            Console.WriteLine("    build the modules in the current folder");
            return;
        }
        else
        {
            logger.Log("Starting Command Factory", "Program", LogType.INFO);
            var command = CommandFactory.GetCommand(args[0], version, logger, location);
            if (command == null) { return; }

            if (argumentsList.Contains("-h") || argumentsList.Contains("--help"))
            {
                logger.Log("Running Help Command", "Program", LogType.INFO);
                command.Help();
            }
            else
            {
                try
                {
                    var arguments = argumentsList.ToArray();
                    command.Run(arguments);
                    logger.Log("Command completed succesfully", "Program", LogType.INFO);
                }
                catch (Exception exception)
                {
                    logger.Log(exception.ToString(), "Program", LogType.ERROR);

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n{exception}");
                    Console.ForegroundColor = ConsoleColor.White;
                    throw;
                }
            }
        }
    }
}