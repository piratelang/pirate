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

    [Theory]
    [InlineData("main.pirate", PirateFileExtensionKind.Module, "main")]
    [InlineData("data.pir", PirateFileExtensionKind.Module, "data")]
    [InlineData("Counter.cpirate", PirateFileExtensionKind.Class, "Counter")]
    [InlineData("Counter.cpir", PirateFileExtensionKind.Class, "Counter")]
    [InlineData("Shape.ipirate", PirateFileExtensionKind.Interface, "Shape")]
    [InlineData("Shape.ipir", PirateFileExtensionKind.Interface, "Shape")]
    public void DiscoverSourceFiles_ClassifiesEveryExtension(string fileName, PirateFileExtensionKind expectedKind, string expectedTypeName)
    {
        System.IO.File.WriteAllText(Path.Combine(_root.FullName, fileName), string.Empty);

        var result = PirateFileLocator.DiscoverSourceFiles(_root.FullName);

        var file = Assert.Single(result);
        Assert.Equal(expectedKind, file.Kind);
        Assert.Equal(expectedTypeName, file.TypeName);
    }

    [Fact]
    public void DiscoverSourceFiles_DoesNotConfuseShortAndLongExtensions()
    {
        // A ".pir" file must not also be picked up as ".pirate" (or vice
        // versa) via the Windows three-character-pattern glob quirk this
        // discovery sidesteps by comparing the real suffix.
        System.IO.File.WriteAllText(Path.Combine(_root.FullName, "main.pirate"), string.Empty);
        System.IO.File.WriteAllText(Path.Combine(_root.FullName, "other.pir"), string.Empty);

        var result = PirateFileLocator.DiscoverSourceFiles(_root.FullName);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, f => f.TypeName == "main" && f.Kind == PirateFileExtensionKind.Module);
        Assert.Contains(result, f => f.TypeName == "other" && f.Kind == PirateFileExtensionKind.Module);
    }

    [Fact]
    public void DiscoverSourceFiles_DottedTypeSegmentIsReportedAsIs()
    {
        // foo.bar.cpirate is a duplicate-name/invalid-name concern for a
        // higher-level caller (the project type registry) — discovery just
        // reports what the filename spells.
        System.IO.File.WriteAllText(Path.Combine(_root.FullName, "foo.bar.cpirate"), string.Empty);

        var result = PirateFileLocator.DiscoverSourceFiles(_root.FullName);

        var file = Assert.Single(result);
        Assert.Equal("foo.bar", file.TypeName);
    }
}
