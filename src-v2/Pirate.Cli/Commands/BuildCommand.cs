using System.ComponentModel;
using System.Threading;
using Pirate.Shared.File;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Pirate.Cli.Commands;

/// <summary>
/// pirate build [filename] — v2 equivalent of v1's BuildCommand. Discovers
/// ".pirate" modules in the current directory, or resolves a single named
/// one (real, working today); compiling them through the
/// lexer/parser/semantics/compiler pipeline is not implemented yet — see
/// docs/architecture/v2-architecture.md for pipeline status.
/// </summary>
public sealed class BuildCommand : Command<BuildCommand.BuildCommandSettings>
{
    public sealed class BuildCommandSettings : GlobalSettings
    {
        [CommandArgument(0, "[filename]")]
        [Description("Optional filename to build; if not provided, all .pirate files in the current directory will be discovered and built.")]
        [DefaultValue(null)]
        public string? Filename { get; set; }
    }

    protected override int Execute(CommandContext context, BuildCommandSettings settings, CancellationToken cancellationToken)
    {
        var root = Directory.GetCurrentDirectory();

        IReadOnlyList<string> files;
        if (string.IsNullOrWhiteSpace(settings.Filename))
        {
            files = PirateFileLocator.DiscoverPirateFiles(root);
            if (files.Count == 0)
            {
                AnsiConsole.MarkupLine($"[{Theme.Error}]No .pirate files were found in the current directory.[/]");
                return 1;
            }
        }
        else
        {
            var name = PirateFileName.Resolve(settings.Filename);
            var path = Path.Combine(root, $"{name}.pirate");
            if (!File.Exists(path))
            {
                AnsiConsole.MarkupLine($"[{Theme.Error}]File \"{Markup.Escape(name)}.pirate\" not provided or does not exist.[/]");
                return 1;
            }
            files = new[] { path };
        }

        // Same look as the no-args banner's command list and "new"'s options
        // table (Banner.cs, NewCommand.cs): a Rule divider over a borderless,
        // bold-row Table, instead of a bordered ASCII grid.
        AnsiConsole.Write(new Rule($"[{Theme.Info}]Discovered modules[/]").LeftJustified());

        var table = new Table().Border(TableBorder.None).HideHeaders();
        table.AddColumn(string.Empty);
        AnsiConsole.Progress().Start(ctx =>
        {
            foreach (var file in files)
            {
                var relative = Path.GetRelativePath(root, file);
                var task = ctx.AddTask(Markup.Escape(relative));
                table.AddRow($"[bold]{Markup.Escape(relative)}[/]");
                task.Increment(100);
            }
        });
        AnsiConsole.Write(table);

        AnsiConsole.MarkupLine(
            $"[{Theme.Warning}]Compilation is not implemented yet — the v2 lexer/parser/semantics/compiler " +
            "pipeline is still a stub (see docs/architecture/v2-architecture.md).[/]");
        return 0;
    }
}
