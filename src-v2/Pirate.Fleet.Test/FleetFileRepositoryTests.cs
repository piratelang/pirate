using System.Text.Json;
using Pirate.Fleet;
using Xunit;

namespace Pirate.Fleet.Test;

public class FleetFileRepositoryTests : IDisposable
{
    private readonly DirectoryInfo _root;

    public FleetFileRepositoryTests()
    {
        _root = Directory.CreateTempSubdirectory("pirate-fleet-test-");
    }

    public void Dispose()
    {
        _root.Delete(recursive: true);
    }

    [Fact]
    public void Exists_NoFleetFile_ReturnsFalse()
    {
        Assert.False(FleetFileRepository.Exists(_root.FullName));
    }

    [Fact]
    public void TryRead_NoFleetFile_ReturnsNull()
    {
        Assert.Null(FleetFileRepository.TryRead(_root.FullName));
    }

    [Fact]
    public void WriteThenRead_RoundTripsFields()
    {
        var fleet = new FleetFile { Name = "my-project", Version = "1.2.3", EntryPoint = "start" };

        FleetFileRepository.Write(_root.FullName, "module", fleet);
        var result = FleetFileRepository.TryRead(_root.FullName);

        Assert.True(FleetFileRepository.Exists(_root.FullName));
        Assert.True(File.Exists(Path.Combine(_root.FullName, "module.fleet")));
        Assert.NotNull(result);
        Assert.Equal("my-project", result!.Name);
        Assert.Equal("1.2.3", result.Version);
        Assert.Equal("start", result.EntryPoint);
    }

    [Fact]
    public void Write_UsesGivenName()
    {
        FleetFileRepository.Write(_root.FullName, "my-project", new FleetFile());

        Assert.True(File.Exists(Path.Combine(_root.FullName, "my-project.fleet")));
    }

    [Fact]
    public void TryRead_MalformedJson_ThrowsWithFileNameInMessage()
    {
        File.WriteAllText(Path.Combine(_root.FullName, "module.fleet"), "{ not valid json");

        var ex = Assert.Throws<JsonException>(() => FleetFileRepository.TryRead(_root.FullName));
        Assert.Contains("module.fleet", ex.Message);
    }
}
