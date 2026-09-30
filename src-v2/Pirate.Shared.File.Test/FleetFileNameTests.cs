using Pirate.Shared.File;
using Xunit;

namespace Pirate.Shared.File.Test;

public class FleetFileNameTests
{
    [Fact]
    public void Resolve_NullArgument_ReturnsModule()
    {
        Assert.Equal("module", FleetFileName.Resolve(null));
    }

    [Fact]
    public void Resolve_EmptyArgument_ReturnsModule()
    {
        Assert.Equal("module", FleetFileName.Resolve(string.Empty));
    }

    [Fact]
    public void Resolve_WhitespaceArgument_ReturnsModule()
    {
        Assert.Equal("module", FleetFileName.Resolve("   "));
    }

    [Fact]
    public void Resolve_NameWithoutExtension_ReturnsNameUnchanged()
    {
        Assert.Equal("my-project", FleetFileName.Resolve("my-project"));
    }

    [Fact]
    public void Resolve_NameWithExtension_StripsExtension()
    {
        Assert.Equal("my-project", FleetFileName.Resolve("my-project.fleet"));
    }

    [Fact]
    public void Resolve_NameWithExtensionDifferentCase_StripsExtension()
    {
        Assert.Equal("my-project", FleetFileName.Resolve("my-project.FLEET"));
    }
}
