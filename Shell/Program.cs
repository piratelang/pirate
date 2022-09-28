using System.Reflection;
using System;
using System.Net.Security;
// See https://aka.ms/new-console-template for more information
using PirateLexer;
using PirateParser;
using PirateInterpreter;

namespace TeleprompterConsole;

internal class Program
{
    static void Main(string[] args)
    {
        /*
            run = pirate run [file].pirate
            init = pirate init
        */

        var version = "0.1.0";
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
                if (args.Length == 1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nFile not provided or does not exist.");
                    Console.ForegroundColor = ConsoleColor.White;

                    Console.WriteLine("\nThe \"pirate run [filename]\" command runs the specified module");
                }
                else
                {
                    var fileName = args[1].Replace(".pirate", "");
                    Run(version, $"{fileName}.pirate");
                }
            }
            if (args[0] == "new")
            {
                if (args.Length == 1)
                {
                    Console.WriteLine("\nThe \"pirate new [type]\" command creates a new file from a template");
                    Console.WriteLine("\nOptions");
                    Console.WriteLine(" - gitignore");
                    Console.WriteLine(" - gitattributes");
                    Console.WriteLine("\nFor a new pirate file see \"pirate init\" ");
                }
                else
                {
                    New(args[1]);
                    Console.WriteLine($"\nCreated new .{args[1]}");
                }
            }
            if (args[0] == "init")
            {
                var fileName = "main";
                if (args.Length > 1)
                {
                    fileName.Replace(".pirate", "");
                    fileName = args[1];
                }
                var file = File.CreateText($"./{fileName}.pirate");
                file.Write("func main()\n{\nprint(\"Hello World\");\n}");
                file.Close();
                Console.WriteLine($"\nCreated {fileName}.pirate");
            }
        }
    }

    static void Run(string version, string file)
    {

        var location = $"bin/pirate{version}";

        var text = File.ReadAllText(file);
        var lexer = new Lexer("test", text);
        var tokens = lexer.MakeTokens();

        var parser = new Parser(tokens.tokens);
        parser.Parse(location);

        var pythonEngine = new PythonEngine(location);
        var result = pythonEngine.InvokeMain("main");
    }

    static void New(string type)
    {
        if (type == "gitignore")
        {
            var file = File.CreateText($"./.gitignore");
            file.Write("[Bb]in/");
            file.Close();
        }
        if (type == "gitattributes")
        {
            var location = "./.gitattributes";
            var file = File.CreateText(location);
            file.Write("*.pirate linguist-language=Squirrel");
            file.Close();
        }
    }
}