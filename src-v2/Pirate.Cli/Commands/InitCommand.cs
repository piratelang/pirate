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
    public sealed class InitCommandSettings : GlobalSettings
    {
        [CommandArgument(0, "[filename]")]
        public string? FileName { get; set; }

        [CommandOption("-n|--name")]
        [Description("Name for the \".fleet\" manifest file, without extension; defaults to \"module\".")]
        [DefaultValue(null)]
        public string? FleetName { get; set; }
    }

    protected override int Execute(CommandContext context, InitCommandSettings settings, CancellationToken cancellationToken)
    {
        var name = PirateFileName.Resolve(settings.FileName);
        File.WriteAllText($"{name}.pirate", Templates.HelloWorldPirate);
        AnsiConsole.MarkupLine($"\n[{Theme.Success}]Created {Markup.Escape(name)}.pirate[/]");

        var directory = Directory.GetCurrentDirectory();
        if (FleetFileRepository.Exists(directory))
        {
            AnsiConsole.MarkupLine($"[{Theme.Error}]A \".fleet\" file already exists[/]");
            return 1;
        }

        var fleetName = FleetFileName.Resolve(settings.FleetName);
        FleetFileRepository.Write(directory, fleetName, new FleetFile
        {
            Name = Path.GetFileName(directory),
            EntryPoint = name,
        });
        AnsiConsole.MarkupLine($"[{Theme.Success}]Created {Markup.Escape(fleetName)}.fleet[/]");

        return 0;
    }
}
