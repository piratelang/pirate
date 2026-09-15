using Pirate.Shared.File;
using Xunit;

namespace Pirate.Shared.File.Test;

public class PirateFileLocatorTests : IDisposable
{
    private readonly DirectoryInfo _root;

    public PirateFileLocatorTests()
    {
        _root = Directory.CreateTempSubdirectory("pirate-shared-file-test-");
    }

    public void Dispose()
    {
        _root.Delete(recursive: true);
    }

    [Fact]
    public void DiscoverPirateFiles_EmptyDirectory_ReturnsEmpty()
    {
        var result = PirateFileLocator.DiscoverPirateFiles(_root.FullName);

        Assert.Empty(result);
    }

    [Fact]
    public void DiscoverPirateFiles_IgnoresNonPirateFiles()
    {
        // Fully qualified: this test's own namespace nests under "Pirate.Shared.File",
        // so a bare "File" resolves to that namespace, not System.IO.File.
        System.IO.File.WriteAllText(Path.Combine(_root.FullName, "notes.txt"), string.Empty);
        System.IO.File.WriteAllText(Path.Combine(_root.FullName, "main.pirate"), string.Empty);

        var result = PirateFileLocator.DiscoverPirateFiles(_root.FullName);

        Assert.Single(result);
        Assert.EndsWith("main.pirate", result[0]);
    }

    [Fact]
    public void DiscoverPirateFiles_SearchesSubdirectoriesRecursively()
    {
        var subdirectory = _root.CreateSubdirectory("modules");
        System.IO.File.WriteAllText(Path.Combine(_root.FullName, "main.pirate"), string.Empty);
        System.IO.File.WriteAllText(Path.Combine(subdirectory.FullName, "helper.pirate"), string.Empty);

        var result = PirateFileLocator.DiscoverPirateFiles(_root.FullName);

        Assert.Equal(2, result.Count);
    }
}
