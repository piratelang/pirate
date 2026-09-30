namespace Pirate.Shared.File;

/// <summary>
/// Finds files by extension in a directory. Shared by
/// <see cref="PirateFileLocator"/> (".pirate", recursive) and
/// Pirate.Fleet's FleetFileLocator (".fleet", top-directory-only).
/// </summary>
public static class FileDiscovery
{
    public static IReadOnlyList<string> Discover(string directory, string extension, SearchOption searchOption)
    {
        return Directory
            .GetFiles(directory, $"*{extension}", searchOption)
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToList();
    }
}
