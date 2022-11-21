using Common;
using PirateInterpreter.Interfaces;
using PirateLexer.Interfaces;
using PirateParser;
using Shell.Commands.Interfaces;

namespace Shell.Commands;

public class ShellCommand : Command, ICommand, IShellCommand
{
    private IParser Parser;
    private ILexer Lexer;
    private IInterpreter Interpreter;
    private IEnvironmentVariables EnvironmentVariables;

    public ShellCommand(ILogger logger, IParser parser, ILexer lexer, IInterpreter interpreter, IEnvironmentVariables environmentVariables) : base(logger, environmentVariables)
    {
        Parser = parser;
        Lexer = lexer;
        Interpreter = interpreter;
        EnvironmentVariables = environmentVariables;
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
                if (tokens.Count() == 0) Error($"Error occured while lexing tokens.");

                var parseResult = Parser.StartParse(tokens, "repl");
                if (parseResult.Nodes.Count() < 1) Error("Error occured while parsing tokens.");

                var interpreterResult = Interpreter.StartInterpreter("repl");
                foreach (var item in interpreterResult)
                {
                    if (item == null)
                    {
                        Logger.Log("Interpreter returned null", this.GetType().Name, LogType.ERROR);
                        continue;
                    }
                    if (item.Value is not null)
                    {
                        Console.WriteLine(item.Value.ToString());
                    }
                    else
                    {
                        Console.WriteLine("item is null");
                    }
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
