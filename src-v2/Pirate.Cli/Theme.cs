using Spectre.Console;

namespace Pirate.Cli;

/// <summary>Shared markup colors so every command agrees on what "error"/"warning"/etc. look like.</summary>
internal static class Theme
{
    public const string Error = "red";
    public const string Warning = "yellow";
    public const string Success = "green";
    public const string Info = "grey";

    public static readonly Color Accent = Color.Gold1;
}
