namespace Pirate.Shared.File;

/// <summary>
/// Finds ".pirate" source files under a directory, recursively — used by the
/// build command to discover the modules of a project, matching v1's
/// <c>Directory.GetFiles("./", "*.pirate", SearchOption.AllDirectories)</c>.
/// </summary>
public static class PirateFileLocator
{
    public static IReadOnlyList<string> DiscoverPirateFiles(string rootDirectory) =>
        FileDiscovery.Discover(rootDirectory, ".pirate", SearchOption.AllDirectories);
}
