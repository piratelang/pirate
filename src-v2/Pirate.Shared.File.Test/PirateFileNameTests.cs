using Pirate.Shared.File;
using Xunit;

namespace Pirate.Shared.File.Test;

public class PirateFileNameTests
{
    [Fact]
    public void Resolve_NullArgument_ReturnsMain()
    {
        var result = PirateFileName.Resolve(null);

        Assert.Equal("main", result);
    }

    [Fact]
    public void Resolve_EmptyArgument_ReturnsMain()
    {
        var result = PirateFileName.Resolve(string.Empty);

        Assert.Equal("main", result);
    }

    [Fact]
    public void Resolve_WhitespaceArgument_ReturnsMain()
    {
        var result = PirateFileName.Resolve("   ");

        Assert.Equal("main", result);
    }

    [Fact]
    public void Resolve_NameWithoutExtension_ReturnsNameUnchanged()
    {
        var result = PirateFileName.Resolve("demo");

        Assert.Equal("demo", result);
    }

    [Fact]
    public void Resolve_NameWithExtension_StripsExtension()
    {
        var result = PirateFileName.Resolve("demo.pirate");

        Assert.Equal("demo", result);
    }

    [Fact]
    public void Resolve_NameWithExtensionDifferentCase_StripsExtension()
    {
        var result = PirateFileName.Resolve("demo.PIRATE");

        Assert.Equal("demo", result);
    }
}
