using System.Reflection;

namespace Pirate.Cli;

/// <summary>
/// Shared CLI metadata. Single source of truth for the version string,
/// used by <c>Banner.cs</c>.
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
