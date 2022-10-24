using Common;
using Common.Enum;
using Interpreter;
using Lexer;
using Parser;
using Shell.Commands.Interfaces;

namespace Shell.Commands;

public class ShellCommand : Command, ICommand, IShellCommand
{
    private IParser Parser;
    private ILexer Lexer;
    private IInterpreter Interpreter;

    public ShellCommand(ILogger logger, IParser parser, ILexer lexer, IInterpreter interpreter) : base(logger)
    {
        Parser = parser;
        Lexer = lexer;
        Interpreter = interpreter;
    }

    public override void Run(string[] arguments)
    {
        Console.WriteLine($"PirateLang version {EnvironmentVariables.GetVariable("version")}");
        Logger.Log("Starting Shell Command", this.GetType().Name, LogType.INFO);
        while (true)
        {
            try
            {
                Console.Write(">> ");
                var input = Console.ReadLine();
                List<string> exitterms = new() { "stop", "exit", "break" };
                if (input == null || input == "")
                {
                    continue;
                }
                if (exitterms.Contains(input))
                {
                    break;
                }

                var tokens = Lexer.MakeTokens(input, "test");
                if (tokens.tokens.Count() == 0) Error($"Error occured while lexing tokens, in the file. {tokens.error.AsString()}");

                var parseResult = Parser.StartParse(tokens.tokens, "repl");
                if (parseResult.Nodes.Count() < 1) Error("Error occured while parsing tokens.");

                var interpreterResult = Interpreter.StartInterpreter("repl");
                foreach (var item in interpreterResult)
                {
                    Console.WriteLine(item.Value);
                }
            }
            catch (Exception ex)
            {
                Error(ex.ToString());
            }
        }
    }

    public override void Help()
    {
        Console.WriteLine(String.Join(
            Environment.NewLine,
            "Description",
            "   pirate repl command",
            "\nUsage",
            "   pirate shell",
            "\nOptions",
            "   -h --help       Show command line help."
        ));
    }

    public override void Error(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n{message}");
        Console.ForegroundColor = ConsoleColor.White;
    }
}
