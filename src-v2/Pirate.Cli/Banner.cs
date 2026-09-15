using Spectre.Console;
using System.Reflection;

namespace Pirate.Cli;

/// <summary>
/// The banner shown when the CLI is invoked with no arguments — mirrors v1's
/// <c>NoCommand</c> (see docs/architecture/v1-architecture.md) but rendered
/// through Spectre.Console (FigletText/Rule/Table) instead of a hand-joined
/// ASCII-art string.
/// </summary>
internal static class Banner
{
    public static readonly IReadOnlyList<(string Usage, string Description)> Commands = new[]
    {
        ("run [filename]", "run the specified file"),
        ("init [filename]", "initializes a new pirate project"),
        ("new [type] [filename]", "create a new file from a template"),
        ("build", "build the modules in the current folder"),
        ("shell", "opens the pirate repl"),
    };

    public static void Render()
    {
        AnsiConsole.Write(new FigletText("Pirate").Color(Theme.Accent));
        AnsiConsole.MarkupLine($"[{Theme.Info}]PirateLang version {Markup.Escape(Version())}[/]");
        AnsiConsole.WriteLine();

        AnsiConsole.Write(new Rule($"[{Theme.Warning}]Commands[/]").LeftJustified());

        var table = new Table().Border(TableBorder.None).HideHeaders();
        table.AddColumn(string.Empty);
        table.AddColumn(string.Empty);
        foreach (var (usage, description) in Commands)
        {
            table.AddRow($"[bold]pirate {Markup.Escape(usage)}[/]", Markup.Escape(description));
        }
        AnsiConsole.Write(table);

    }
    
    private static string Version()
    {
        return Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion
        ?? "unknown";
    }
}
