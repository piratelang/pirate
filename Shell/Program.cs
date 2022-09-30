using System.Runtime.InteropServices;
using System.Reflection;
using System;
using System.Net.Security;
// See https://aka.ms/new-console-template for more information
using PirateLexer;
using PirateParser;
using PirateInterpreter;
using Shell.Commands;

namespace Shell;

internal class Program
{
    static void Main(string[] args)
    {
        var version = "0.1.1";
        List<string> argumentsList = new();

        foreach(var item in args)
        {
            if(item != null)
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
            Console.WriteLine(" - pirate init [filename]\n");
            Console.WriteLine("    initializes a new pirate project");
            Console.WriteLine(" - pirate new [type]\n");
            Console.WriteLine("    create a new file");
            return;
        }
        else
        {
            var command = CommandFactory.GetCommand(args[0], version);
            if (command == null){ return; }

            if (argumentsList.Contains("-h") || argumentsList.Contains("--help"))
            {
                command.Help();
            }
            else
            {
                var arguments = argumentsList.ToArray();
                command.Run(arguments);
            }
        }
    }
}