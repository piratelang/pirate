using System.ComponentModel;
using System.Threading;
using Pirate.Cli.Services;
using Pirate.Fleet;
using Pirate.Shared.File;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Pirate.Cli.Commands;

/// <summary>pirate init [filename] — v2 equivalent of v1's InitCommand.</summary>
public sealed class InitCommand : Command<InitCommand.InitCommandSettings>
{
    /// <summary>
    /// Root namespaces the language itself owns (docs/GRAMMAR.md §3.2's
    /// <c>import standard &lt;NS&gt;;</c>) — a project can't claim one as
    /// its own root namespace, or every module in it would collide with
    /// the builtin group of the same name.
    /// </summary>
    private static readonly HashSet<string> ReservedRoots = new(StringComparer.OrdinalIgnoreCase) { "standard" };

    public sealed class InitCommandSettings : GlobalSettings
    {
        [CommandArgument(0, "[filename]")]
        public string? FileName { get; set; }

        [CommandOption("-n|--name")]
        [Description("Project name — both the \".fleet\" file name and the root namespace; prompted for when omitted.")]
        [DefaultValue(null)]
        public string? FleetName { get; set; }
    }

    protected override int Execute(CommandContext context, InitCommandSettings settings, CancellationToken cancellationToken)
    {
        var directory = Directory.GetCurrentDirectory();

        // The project name is both the ".fleet" file's base name and the
        // root namespace every type in the project resolves under
        // (GRAMMAR.md §4.4) — one source of truth, so '-n' (or the prompt)
        // supplies a single name instead of the old decoupled
        // folder-name-for-namespace / "module"-for-filename defaults.
        string projectName;
        if (!string.IsNullOrWhiteSpace(settings.FleetName))
        {
            projectName = settings.FleetName;
        }
        else
        {
            projectName = AnsiConsole.Prompt(
                new TextPrompt<string>("Project name:")
                    .DefaultValue(Path.GetFileName(directory))
                    .Validate(ValidateProjectName));
        }

        if (ValidateProjectName(projectName) is { Successful: false } invalid)
        {
            AnsiConsole.MarkupLine($"[{Theme.Error}]{Markup.Escape(invalid.Message ?? "Invalid project name")}[/]");
            return 1;
        }

        var name = PirateFileName.Resolve(settings.FileName);
        File.WriteAllText($"{name}.pirate", Templates.HelloWorldPirate);
        AnsiConsole.MarkupLine($"\n[{Theme.Success}]Created {Markup.Escape(name)}.pirate[/]");

        if (FleetFileRepository.Exists(directory))
        {
            AnsiConsole.MarkupLine($"[{Theme.Error}]A \".fleet\" file already exists[/]");
            return 1;
        }

        FleetFileRepository.Write(directory, projectName, new FleetFile
        {
            Name = projectName,
            EntryPoint = name,
        });
        AnsiConsole.MarkupLine($"[{Theme.Success}]Created {Markup.Escape(projectName)}.fleet[/]");

        return 0;
    }

    private static ValidationResult ValidateProjectName(string candidate) =>
        ReservedRoots.Contains(candidate)
            ? ValidationResult.Error($"'{candidate}' is a reserved root and can't be a project name")
            : ValidationResult.Success();
}
