using Pirate.Cli.Commands;
using Xunit;

namespace Pirate.Cli.Test.Commands;

public class NewCommandSettingsTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("list")]
    public void Validate_NoTypeOrList_Succeeds(string? type)
    {
        var settings = new NewCommand.NewCommandSettings { Type = type };

        Assert.True(settings.Validate().Successful);
    }

    [Theory]
    [InlineData("pirate")]
    [InlineData("gitignore")]
    [InlineData("gitattributes")]
    public void Validate_KnownType_Succeeds(string type)
    {
        var settings = new NewCommand.NewCommandSettings { Type = type };

        Assert.True(settings.Validate().Successful);
    }

    [Fact]
    public void Validate_UnknownType_FailsWithMessage()
    {
        var settings = new NewCommand.NewCommandSettings { Type = "bogus" };

        var result = settings.Validate();

        Assert.False(result.Successful);
        Assert.Contains("bogus", result.Message);
    }
}
