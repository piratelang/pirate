namespace Pirate.Shared.File;

/// <summary>
/// Finds "*.fleet" manifest files directly inside a directory — not
/// recursive, unlike <see cref="PirateFileLocator"/>, since a manifest
/// identifies a project root rather than a module anywhere in a tree.
/// </summary>
public static class FleetFileLocator
{
    public static IReadOnlyList<string> DiscoverFleetFiles(string directory) =>
        FileDiscovery.Discover(directory, ".fleet", SearchOption.TopDirectoryOnly);
}
