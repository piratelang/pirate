namespace Pirate.Shared.File;

/// <summary>
/// Finds Pirate source files under a directory, recursively — used by the
/// build command to discover a project's modules and class/interface
/// files. Knows all six extensions (docs/GRAMMAR.md §4): <c>.pirate</c>,
/// <c>.pir</c> (module), <c>.cpirate</c>, <c>.cpir</c> (class),
/// <c>.ipirate</c>, <c>.ipir</c> (interface).
/// </summary>
public static class PirateFileLocator
{
    private static readonly (string Extension, PirateFileExtensionKind Kind)[] Extensions =
    [
        (".cpirate", PirateFileExtensionKind.Class),
        (".cpir", PirateFileExtensionKind.Class),
        (".ipirate", PirateFileExtensionKind.Interface),
        (".ipir", PirateFileExtensionKind.Interface),
        (".pirate", PirateFileExtensionKind.Module),
        (".pir", PirateFileExtensionKind.Module),
    ];

    /// <summary>
    /// Paths only, every kind mixed together — the shape most existing
    /// callers (the build cache's watch list, "how many files") need.
    /// </summary>
    public static IReadOnlyList<string> DiscoverPirateFiles(string rootDirectory) =>
        [.. DiscoverSourceFiles(rootDirectory).Select(file => file.Path)];

    /// <summary>
    /// Every source file under <paramref name="rootDirectory"/>, with its
    /// kind and type name. <c>Directory.GetFiles(dir, "*.pir")</c> also
    /// matches <c>.pirate</c> on .NET's documented three-character-pattern
    /// behavior, so this enumerates every file once and compares the real
    /// suffix itself instead of globbing per extension — each of the six
    /// extensions ends in a different character right before a shared
    /// tail (<c>.cpir</c> vs <c>.pir</c>, etc.), so an exact,
    /// case-insensitive suffix match never double-counts a file.
    /// </summary>
    public static IReadOnlyList<PirateSourceFile> DiscoverSourceFiles(string rootDirectory)
    {
        var matches = new List<PirateSourceFile>();
        foreach (var path in Directory.GetFiles(rootDirectory, "*.*", SearchOption.AllDirectories))
        {
            foreach (var (extension, kind) in Extensions)
            {
                if (!path.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var fileName = Path.GetFileName(path);
                var typeName = fileName[..^extension.Length];
                matches.Add(new PirateSourceFile(path, kind, typeName));
                break;
            }
        }

        return [.. matches.OrderBy(file => file.Path, StringComparer.Ordinal)];
    }
}
