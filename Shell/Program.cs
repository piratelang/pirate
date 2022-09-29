using System.Reflection;
using System;
using System.Net.Security;
// See https://aka.ms/new-console-template for more information
using PirateLexer;
using PirateParser;
using PirateInterpreter;

namespace Shell;

internal class Program
{
    static void Main(string[] args)
    {
        var version = "0.1.0";
        var commandRepository = new CommandRepository(version);

        if (args.Length == 0)
        {
            Console.WriteLine($"\nPirateLang version {version}\n");
            Console.WriteLine("Commands:");
            Console.WriteLine(" - pirate run [filename].pirate");
            Console.WriteLine("    run the specified file");
            Console.WriteLine(" - pirate init [filename]\n");
            Console.WriteLine("    initialize a new pirste project");
            Console.WriteLine(" - pirate new [type]\n");
            Console.WriteLine("    initialize a new pirste project");
            return;
        }
        else
        {
            if (args[0] == "run")
            {
                var fileArgument = string.Empty;
                try
                {
                    fileArgument = args[1];
                }
                catch { }
                commandRepository.Run(fileArgument);
            }
            if (args[0] == "new")
            {
                var typeArgument = string.Empty;
                try
                {
                    typeArgument = args[1];
                }
                catch (System.Exception) { }
                commandRepository.New(typeArgument);

            }
            if (args[0] == "init")
            {
                var nameArgument = string.Empty;
                try
                {
                    nameArgument = args[1];
                }
                catch (System.Exception) { }

                commandRepository.Init(nameArgument);
            }
        }
    }
}