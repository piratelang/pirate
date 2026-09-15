using System.Text.Json;
using System.Threading;
using Pirate.Fleet;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Pirate.Cli.Commands;

/// <summary>
/// pirate run [filename] — v2 equivalent of v1's RunCommand. File resolution
/// and existence checks are real (matching v1's behavior); executing the
/// program is not implemented yet — see docs/architecture/v2-architecture.md
/// for pipeline status. With no [filename], resolves to the ".fleet"
/// manifest's declared entry point when one exists (see FleetEntryPoint),
/// falling back to "main" otherwise.
/// </summary>
public sealed class RunCommand : Command<RunCommand.RunCommandSettings>
{
    public sealed class RunCommandSettings : GlobalSettings
    {
        [CommandArgument(0, "[filename]")]
        public string? FileName { get; set; }
    }

    protected override int Execute(CommandContext context, RunCommandSettings settings, CancellationToken cancellationToken)
    {
        string name;
        try
        {
            name = FleetEntryPoint.Resolve(settings.FileName, Directory.GetCurrentDirectory());
        }
        catch (JsonException ex)
        {
            AnsiConsole.MarkupLine($"[{Theme.Error}]A \".fleet\" file exists but could not be read: {Markup.Escape(ex.Message)}[/]");
            return 1;
        }

        var path = $"{name}.pirate";

        var found = false;
        AnsiConsole.Status().Start($"Resolving {Markup.Escape(path)}...", ctx =>
        {
            found = File.Exists(path);
        });

        if (!found)
        {
            AnsiConsole.MarkupLine($"[{Theme.Error}]File \"{Markup.Escape(name)}.pirate\" not provided or does not exist.[/]");
            return 1;
        }

        AnsiConsole.MarkupLine(
            $"[{Theme.Warning}]Found {Markup.Escape(path)}, but execution is not implemented yet — the v2 lexer/parser/" +
            "semantics/compiler/VM pipeline is still a stub (see docs/architecture/v2-architecture.md).[/]");
        return 0;
    }
}
