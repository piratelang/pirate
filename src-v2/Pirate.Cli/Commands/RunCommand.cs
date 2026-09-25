using System.Text.Json;
using System.Threading;
using Pirate.Cli.Services;
using Pirate.Fleet;
using Pirate.Shared.File;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Pirate.Cli.Commands;

/// <summary>
/// pirate run [filename] — builds changed modules (content-hash based),
/// then executes through the VM (still stub).
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
        var root = Directory.GetCurrentDirectory();
        var cache = BuildCache.Load(root);

        string name;
        try
        {
            name = FleetEntryPoint.Resolve(settings.FileName, root);
        }
        catch (JsonException ex)
        {
            AnsiConsole.MarkupLine($"[{Theme.Error}]A \".fleet\" file exists but could not be read: {Markup.Escape(ex.Message)}[/]");
            return 1;
        }

        var path = Path.Combine(root, $"{name}.pirate");

        if (!File.Exists(path))
        {
            AnsiConsole.MarkupLine($"[{Theme.Error}]File \"{Markup.Escape(name)}.pirate\" not provided or does not exist.[/]");
            return 1;
        }

        var failed = false;
        AnsiConsole.Status().Start($"Resolving {Markup.Escape(name)}.pirate...", ctx =>
        {
            AnsiConsole.WriteLine();

            // Build phase
            AnsiConsole.Write(new Rule($"[{Theme.Info}]Build[/]").LeftJustified());

            if (cache.IsUpToDate(path))
            {
                AnsiConsole.MarkupLine($"  [{Theme.Success}]✓[/] {Markup.Escape(Path.GetRelativePath(root, path))} (up to date)");
            }
            else
            {
                var source = File.ReadAllText(path);
                // Fully qualified: inside Pirate.* namespaces, the simple
                // names Lexer/Parser bind to the namespaces, not the classes.
                var lexResult = Pirate.Lexer.Lexer.Tokenize(source);
                var parseResult = Pirate.Parser.Parser.Parse(lexResult);

                var allErrors = new List<Pirate.Syntax.CompilationError>(lexResult.Errors);
                allErrors.AddRange(parseResult.Errors);

                if (allErrors.Count > 0)
                {
                    AnsiConsole.MarkupLine($"  [{Theme.Error}]✗[/] {Markup.Escape(Path.GetRelativePath(root, path))} (failed)");
                    DiagnosticRenderer.RenderErrors(path, allErrors);
                    failed = true;
                    return;
                }

                cache.MarkBuilt(path);
                AnsiConsole.MarkupLine($"  [{Theme.Success}]⟳[/] {Markup.Escape(Path.GetRelativePath(root, path))} (rebuilt)");
            }

            cache.Save();

            AnsiConsole.WriteLine();
            AnsiConsole.Write(new Rule($"[{Theme.Warning}]Running[/]").LeftJustified());
            AnsiConsole.MarkupLine($"[{Theme.Warning}]Execution is not implemented yet — the v2 VM pipeline is still a stub.[/]");
        });

        return failed ? 1 : 0;
    }
}
