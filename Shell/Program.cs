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
            Console.WriteLine(" - pirate init [filename]\n");
            return;
        }
        else
        {
            if (args[0] == "run")
            {
                if (args.Length == 1)
                {
                    Console.WriteLine("File not provided or does not exist.");
                }
                else
                {
                    var fileName = args[1].Replace(".pirate", "");
                    Run(version, $"{fileName}.pirate");
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
                File.Create($"./{fileName}.pirate");
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
}