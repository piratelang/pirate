using System.Reflection;

namespace Pirate.Cli;

/// <summary>
/// Shared CLI metadata. Single source of truth for the version string
/// (eliminating the duplication between <c>Banner.cs</c> and
/// <c>ShellCommand.cs</c>).
/// </summary>
public static class CliInfo
{
    public static string Version()
    {
        return Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion
            ?? "unknown";
    }
}
