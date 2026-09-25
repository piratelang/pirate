using System.Threading;
using Pirate.Cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Pirate.Cli.Commands;

/// <summary>
/// pirate shell — REPL that lexes and parses each line, showing errors
/// with source excerpts. Execution is stubbed until the VM is implemented.
/// </summary>
public sealed class ShellCommand : Command<ShellCommand.ShellCommandSettings>
{
    public sealed class ShellCommandSettings : GlobalSettings
    {
    }

    private static readonly string[] ExitTerms = { "stop", "exit", "break" };

    protected override int Execute(CommandContext context, ShellCommandSettings settings, CancellationToken cancellationToken)
    {
        AnsiConsole.MarkupLine($"PirateLang version {CliInfo.Version()}");

        var lineNum = 0;
        while (true)
        {
            lineNum++;
            Console.Write(">> ");
            var input = Console.ReadLine();
            if (input is null)
            {
                // EOF (e.g. piped stdin closed) — stop instead of looping forever on a null read.
                break;
            }
            if (input.Length == 0)
            {
                continue;
            }
            if (ExitTerms.Contains(input))
            {
                break;
            }

            // Lex and parse each line. Fully qualified: inside Pirate.*
            // namespaces, the simple names Lexer/Parser bind to the
            // namespaces, not the classes.
            var lexResult = Pirate.Lexer.Lexer.Tokenize(input);
            var parseResult = Pirate.Parser.Parser.Parse(lexResult);

            var allErrors = new List<Pirate.Syntax.CompilationError>(lexResult.Errors);
            allErrors.AddRange(parseResult.Errors);

            if (allErrors.Count > 0)
            {
                foreach (var error in allErrors)
                {
                    var code = ErrorMapper.Map(error);
                    var header = $"stdin:{lineNum}:{error.StartLocation.Column} {error.Message} *{code}*";
                    AnsiConsole.MarkupLine($"[{Theme.Error}]{Markup.Escape(header)}[/]");
                    // Show the offending line
                    var col = Math.Max(0, error.StartLocation.Column - 1);
                    // Caret indent derives from the rendered prefix so it stays
                    // aligned as line numbers widen past 9.
                    var prefix = $"  {lineNum} | ";
                    AnsiConsole.MarkupLine($"[{Theme.Info}]{Markup.Escape(prefix)}{Markup.Escape(input)}[/]");
                    var caret = new string(' ', prefix.Length + col) + "^";
                    AnsiConsole.MarkupLine($"[{Theme.Error}]{caret}[/]");
                }
                AnsiConsole.WriteLine();
            }
            else
            {
                // Parse succeeded — execution is still stub
                AnsiConsole.MarkupLine($"[{Theme.Warning}](parsed successfully, but execution is not implemented yet)[/]");
            }
        }

        return 0;
    }
}
