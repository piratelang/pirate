using Spectre.Console.Cli;
using System.ComponentModel;

namespace Pirate.Cli;

public class GlobalSettings : CommandSettings
{
    [CommandOption("-v|--verbose")]
    [Description("Show detailed build information, including hash changes and all diagnostics.")]
    public bool Verbose { get; set; }
}
