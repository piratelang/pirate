// See https://aka.ms/new-console-template for more information
using PirateLexer;
using PirateParser;
using PirateInterpreter;

namespace TeleprompterConsole;

internal class Program
{
    static void Main(string[] args)
    {

        var text = File.ReadAllText(args[0]);
        var lexer = new Lexer("test", text);
        var tokens = lexer.MakeTokens();

        var parser = new Parser(tokens.tokens);
        parser.Parse();

        var pythonEngine = new PythonEngine("./output.py");
        var result = pythonEngine.InvokeMain("main");

        Console.ReadLine();
    }
}