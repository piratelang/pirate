using System.Threading;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Reflection;

namespace Pirate.Cli.Commands;

/// <summary>
/// pirate shell — v2 equivalent of v1's ShellCommand (REPL). The read/exit
/// loop is real; lexing/parsing/executing each line is not implemented yet —
/// see docs/architecture/v2-architecture.md for pipeline status.
/// </summary>
public sealed class ShellCommand : Command<ShellCommand.ShellCommandSettings>
{
    public sealed class ShellCommandSettings : GlobalSettings
    {
    }

    private static readonly string[] ExitTerms = { "stop", "exit", "break" };

    protected override int Execute(CommandContext context, ShellCommandSettings settings, CancellationToken cancellationToken)
    {
        AnsiConsole.MarkupLine($"PirateLang version {Version()}");

        while (true)
        {
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

            AnsiConsole.MarkupLine(
                $"[{Theme.Warning}]The REPL pipeline is not implemented yet — lexing/parsing/execution is still a " +
                "stub (see docs/architecture/v2-architecture.md).[/]");
        }

        return 0;
    }

    private static string Version()
    {
        return Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion
        ?? "unknown";
    }
}
