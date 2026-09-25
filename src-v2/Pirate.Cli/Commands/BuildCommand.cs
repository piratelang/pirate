using System.ComponentModel;
using System.Threading;
using Pirate.Cli.Services;
using Pirate.Shared.File;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Pirate.Cli.Commands;

/// <summary>
/// pirate build [filename] — discovers .pirate modules, checks each against
/// the content-hash cache, and rebuilds only those that changed.
/// </summary>
public sealed class BuildCommand : Command<BuildCommand.BuildCommandSettings>
{
    public sealed class BuildCommandSettings : GlobalSettings
    {
        [CommandArgument(0, "[filename]")]
        [Description("Optional filename to build; if not provided, all .pirate files in the current directory will be discovered.")]
        [DefaultValue(null)]
        public string? Filename { get; set; }
    }

    protected override int Execute(CommandContext context, BuildCommandSettings settings, CancellationToken cancellationToken)
    {
        var root = Directory.GetCurrentDirectory();
        var cache = BuildCache.Load(root);

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

        var rebuilt = 0;
        var upToDate = 0;
        var failed = 0;

        AnsiConsole.Status().Start("Resolving modules...", ctx =>
        {
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new Rule($"[{Theme.Info}]Build[/]").LeftJustified());

            foreach (var file in files)
            {
                var relative = Path.GetRelativePath(root, file);

                if (cache.IsUpToDate(file))
                {
                    upToDate++;
                    AnsiConsole.MarkupLine($"  [{Theme.Success}]✓[/] {Markup.Escape(relative)} (up to date)");
                    if (settings.Verbose)
                    {
                        AnsiConsole.MarkupLine($"    hash: {BuildCache.HashFile(file)} (unchanged)");
                    }
                }
                else
                {
                    var source = File.ReadAllText(file);
                    // Fully qualified: inside Pirate.* namespaces, the simple
                    // names Lexer/Parser bind to the namespaces, not the classes.
                    var lexResult = Pirate.Lexer.Lexer.Tokenize(source);
                    var parseResult = Pirate.Parser.Parser.Parse(lexResult);

                    var allErrors = new List<Pirate.Syntax.CompilationError>(lexResult.Errors);
                    allErrors.AddRange(parseResult.Errors);

                    if (allErrors.Count > 0)
                    {
                        failed++;
                        AnsiConsole.MarkupLine($"  [{Theme.Error}]✗[/] {Markup.Escape(relative)} (failed)");
                        DiagnosticRenderer.RenderErrors(file, allErrors);
                    }
                    else
                    {
                        rebuilt++;
                        cache.MarkBuilt(file);
                        AnsiConsole.MarkupLine($"  [{Theme.Success}]⟳[/] {Markup.Escape(relative)} (rebuilt)");
                        if (settings.Verbose)
                        {
                            AnsiConsole.MarkupLine($"    hash: {BuildCache.HashFile(file)}");
                        }
                    }
                }
            }
        });

        cache.Save();

        AnsiConsole.WriteLine();
        var summary = $"{files.Count} module{(files.Count == 1 ? "" : "s")}: {rebuilt} rebuilt, {upToDate} up to date";
        if (failed > 0)
        {
            summary += $", {failed} failed";
            AnsiConsole.MarkupLine($"[{Theme.Error}]{summary}[/]");
            return 1;
        }
        AnsiConsole.MarkupLine($"[{Theme.Success}]{summary}[/]");
        return 0;
    }
}
