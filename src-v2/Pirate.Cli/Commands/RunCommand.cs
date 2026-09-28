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
public sealed class RunCommand(ICompilationPipeline compilationPipeline) : Command<RunCommand.RunCommandSettings>
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

            // Always run the frontend: there is no compiled artifact to skip
            // to yet, and the entry-point check needs the checked AST. The
            // cache only decides the rebuilt/up-to-date wording for now.
            var source = File.ReadAllText(path);
            var frontend = compilationPipeline.Compile(source);
            var relative = Path.GetRelativePath(root, path);

            if (!frontend.Success)
            {
                AnsiConsole.MarkupLine($"  [{Theme.Error}]✗[/] {Markup.Escape(relative)} (failed)");
                DiagnosticRenderer.RenderErrors(path, frontend.Errors);
                failed = true;
                return;
            }

            if (cache.IsUpToDate(path))
            {
                AnsiConsole.MarkupLine($"  [{Theme.Success}]✓[/] {Markup.Escape(relative)} (up to date)");
            }
            else
            {
                cache.MarkBuilt(path);
                AnsiConsole.MarkupLine($"  [{Theme.Success}]⟳[/] {Markup.Escape(relative)} (rebuilt)");
            }

            cache.Save();

            // Run phase
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new Rule($"[{Theme.Warning}]Running[/]").LeftJustified());

            // Run-entry rule (GRAMMAR.md §3.1): the entry module's top-level
            // statements are the program; a module without any has nothing
            // to run. RTN-004 when the VM lands.
            if (!EntryPoint.HasRunnableBody(frontend.Program))
            {
                AnsiConsole.MarkupLine($"[{Theme.Error}]Module '{Markup.Escape(name)}.pirate' has no top-level statements — nothing to run.[/]");
                failed = true;
                return;
            }

            AnsiConsole.MarkupLine($"[{Theme.Success}]✓[/] {Markup.Escape(relative)} type-checked clean.");
            AnsiConsole.MarkupLine($"[{Theme.Warning}]Execution is not implemented yet — the v2 VM pipeline is still a stub.[/]");
        });

        return failed ? 1 : 0;
    }
}
