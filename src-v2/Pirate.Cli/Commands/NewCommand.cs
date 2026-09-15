using System.Diagnostics;
using System.Threading;
using Pirate.Cli.Services;
using Pirate.Shared.File;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Pirate.Cli.Commands;

/// <summary>pirate new [type] [filename] — v2 equivalent of v1's NewCommand.</summary>
public sealed class NewCommand : Command<NewCommand.NewCommandSettings>
{
    // Only NewCommand creates these file types, so the list — and what counts
    // as a valid one — lives here rather than in the shared Templates service.
    private static readonly IReadOnlyList<string> NewFileTypes = new[] { "pirate", "gitignore", "gitattributes" };

    private static bool IsValidNewFileType(string type) => NewFileTypes.Contains(type, StringComparer.Ordinal);

    public sealed class NewCommandSettings : GlobalSettings
    {
        [CommandArgument(0, "[type]")]
        public string? Type { get; set; }

        [CommandArgument(1, "[filename]")]
        public string? FileName { get; set; }

        public override ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(Type) || Type == "list")
            {
                // No type, or an explicit "list" — Execute prints the options list,
                // not a validation error.
                return ValidationResult.Success();
            }

            return IsValidNewFileType(Type)
                ? ValidationResult.Success()
                : ValidationResult.Error($"Specified file \"{Type}\" not able to be created");
        }
    }

    protected override int Execute(CommandContext context, NewCommandSettings settings, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(settings.Type) || settings.Type == "list")
        {
            PrintOptions();
            return 0;
        }

        string createdPath;
        switch (settings.Type)
        {
            case "gitignore":
                createdPath = ".gitignore";
                if (File.Exists(createdPath))
                {
                    AnsiConsole.MarkupLine($"[{Theme.Error}]Specified filename \"{createdPath}\" already exists[/]");
                    return 1;
                }
                File.WriteAllText(createdPath, Templates.GitIgnore);
                break;
            case "gitattributes":
                createdPath = ".gitattributes";
                if (File.Exists(createdPath))
                {
                    AnsiConsole.MarkupLine($"[{Theme.Error}]Specified filename \"{createdPath}\" already exists[/]");
                    return 1;
                }
                File.WriteAllText(createdPath, Templates.GitAttributes);
                break;
            case "pirate":
                var name = PirateFileName.Resolve(settings.FileName);
                createdPath = $"{name}.pirate";
                if (File.Exists(createdPath))
                {
                    AnsiConsole.MarkupLine($"[{Theme.Error}]Specified filename \"{Markup.Escape(name)}\" already exists[/]");
                    return 1;
                }
                File.WriteAllText(createdPath, string.Empty);
                break;
            default:
                // Unreachable: NewCommandSettings.Validate() already rejected any other Type.
                throw new UnreachableException($"Unhandled new-file type \"{settings.Type}\".");
        }

        AnsiConsole.MarkupLine($"\n[{Theme.Success}]Created {Markup.Escape(createdPath)}[/]");
        return 0;
    }

    private static void PrintOptions()
    {
        // Plain WriteLine, not MarkupLine: the literal "[type]" below would
        // otherwise be parsed as a (nonexistent) style tag - see docs/CLI.md.
        AnsiConsole.WriteLine("\nThe \"pirate new [type]\" command creates a new file from a template");
        AnsiConsole.WriteLine();

        // Same look as the no-args banner's command list (Banner.cs): a Rule
        // divider over a borderless, bold-row Table, instead of a bordered grid.
        AnsiConsole.Write(new Rule($"[{Theme.Info}]Options[/]").LeftJustified());

        var table = new Table().Border(TableBorder.None).HideHeaders();
        table.AddColumn(string.Empty);
        foreach (var type in NewFileTypes)
        {
            table.AddRow($"[bold]{Markup.Escape(type)}[/]");
        }
        AnsiConsole.Write(table);
    }
}
