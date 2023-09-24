using Pirate.Interpreter.Interfaces;
using Pirate.Lexer;
using Pirate.Parser.Interfaces;
using PirateLang.Commands.Interfaces;
using PirateLang.Commands.Models;

namespace PirateLang.Commands;

/// <summary>
/// A command which starts the pirate repl.
/// </summary>
public class ShellCommand : Command, ICommand, IShellCommand
{
    private IParser Parser;
    private Lexer Lexer;
    private IInterpreter Interpreter;
    private IEnvironmentVariables EnvironmentVariables;

    public ShellCommand(ILogger logger, IParser parser, Lexer lexer, IInterpreter interpreter, IEnvironmentVariables environmentVariables) : base(logger, environmentVariables)
    {
        Parser = parser;
        Lexer = lexer;
        Interpreter = interpreter;
        EnvironmentVariables = environmentVariables;
    }

    public override object Run(string[] arguments)
    {
        Console.WriteLine($"PirateLang version {EnvironmentVariables.GetVariable("version")}");
        Logger.Info("Starting Shell Command");
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

                var tokens = Lexer.MakeTokens(input, "test").ToList();
                if (tokens.Count() == 0) Error($"Error occured while lexing tokens.");

                var parseResult = Parser.StartParse(tokens, "repl");
                if (parseResult.Nodes.Count() < 1) Error("Error occured while parsing tokens.");

                var interpreterResult = Interpreter.StartInterpreter(parseResult);
                foreach (var item in interpreterResult)
                {
                    if (item == null)
                    {
                        Logger.Warning("Interpreter returned null");
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
        return true;
    }

    public override void Help()
    {
        Console.WriteLine(
            new HelpOption(
                description: "pirate repl command",
                usage: "pirate shell",
                options: new List<OptionDescription>()
                {
                    new OptionDescription(
                        options: new List<string>() { "-h", "--help" },
                        description: "Show command line help."
                    )
                }
            ).ToString()
        );
    }

    public override void Error(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n{message}");
        Console.ForegroundColor = ConsoleColor.White;
    }
}
