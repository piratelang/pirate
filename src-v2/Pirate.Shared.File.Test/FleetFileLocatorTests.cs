using Pirate.Shared.File;
using Xunit;

namespace Pirate.Shared.File.Test;

public class FleetFileLocatorTests : IDisposable
{
    private readonly DirectoryInfo _root;

    public FleetFileLocatorTests()
    {
        _root = Directory.CreateTempSubdirectory("pirate-fleet-locator-test-");
    }

    public void Dispose()
    {
        _root.Delete(recursive: true);
    }

    [Fact]
    public void DiscoverFleetFiles_EmptyDirectory_ReturnsEmpty()
    {
        var result = FleetFileLocator.DiscoverFleetFiles(_root.FullName);

        Assert.Empty(result);
    }

    [Fact]
    public void DiscoverFleetFiles_IgnoresNonFleetFiles()
    {
        // Fully qualified: this test's own namespace nests under "Pirate.Shared.File",
        // so a bare "File" resolves to that namespace, not System.IO.File.
        System.IO.File.WriteAllText(Path.Combine(_root.FullName, "notes.txt"), string.Empty);
        System.IO.File.WriteAllText(Path.Combine(_root.FullName, "module.fleet"), string.Empty);

        var result = FleetFileLocator.DiscoverFleetFiles(_root.FullName);

        Assert.Single(result);
        Assert.EndsWith("module.fleet", result[0]);
    }

    [Fact]
    public void DiscoverFleetFiles_DoesNotSearchSubdirectories()
    {
        var subdirectory = _root.CreateSubdirectory("nested");
        System.IO.File.WriteAllText(Path.Combine(subdirectory.FullName, "module.fleet"), string.Empty);

        var result = FleetFileLocator.DiscoverFleetFiles(_root.FullName);

        Assert.Empty(result);
    }

    [Fact]
    public void DiscoverFleetFiles_MultipleFiles_OrdersAlphabetically()
    {
        System.IO.File.WriteAllText(Path.Combine(_root.FullName, "zeta.fleet"), string.Empty);
        System.IO.File.WriteAllText(Path.Combine(_root.FullName, "alpha.fleet"), string.Empty);

        var result = FleetFileLocator.DiscoverFleetFiles(_root.FullName);

        Assert.Equal(2, result.Count);
        Assert.EndsWith("alpha.fleet", result[0]);
        Assert.EndsWith("zeta.fleet", result[1]);
    }
}
