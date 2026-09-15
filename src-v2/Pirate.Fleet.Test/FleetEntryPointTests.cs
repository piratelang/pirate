using Pirate.Fleet;
using Xunit;

namespace Pirate.Fleet.Test;

public class FleetEntryPointTests : IDisposable
{
    private readonly DirectoryInfo _root;

    public FleetEntryPointTests()
    {
        _root = Directory.CreateTempSubdirectory("pirate-fleet-entrypoint-test-");
    }

    public void Dispose()
    {
        _root.Delete(recursive: true);
    }

    [Fact]
    public void Resolve_ExplicitArgument_WinsOverFleetFile()
    {
        FleetFileRepository.Write(_root.FullName, "module", new FleetFile { EntryPoint = "other" });

        var result = FleetEntryPoint.Resolve("explicit", _root.FullName);

        Assert.Equal("explicit", result);
    }

    [Fact]
    public void Resolve_NoArgument_UsesFleetFileEntryPoint()
    {
        FleetFileRepository.Write(_root.FullName, "module", new FleetFile { EntryPoint = "other" });

        var result = FleetEntryPoint.Resolve(null, _root.FullName);

        Assert.Equal("other", result);
    }

    [Fact]
    public void Resolve_NoArgumentAndNoFleetFile_FallsBackToMain()
    {
        var result = FleetEntryPoint.Resolve(null, _root.FullName);

        Assert.Equal("main", result);
    }
}
