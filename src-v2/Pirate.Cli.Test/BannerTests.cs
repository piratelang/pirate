using Pirate.Cli;
using Spectre.Console;
using Spectre.Console.Testing;
using Xunit;

namespace Pirate.Cli.Test;

[Collection(nameof(AnsiConsoleTestCollection))]
public class BannerTests
{
    [Fact]
    public void Render_WritesVersion()
    {
        var output = RenderToString();

        Assert.Contains("PirateLang version", output);
    }

    [Theory]
    [InlineData("run [filename]")]
    [InlineData("init [filename]")]
    [InlineData("new [type] [filename]")]
    [InlineData("build")]
    [InlineData("shell")]
    public void Commands_ListsEveryCommandUsage(string usage)
    {
        Assert.Contains(Banner.Commands, command => command.Usage == usage);
    }

    // Renders through a real Spectre.Console.Testing.TestConsole rather than
    // just asserting on Banner.Commands' data — this is what would have
    // caught the "[type] parsed as a style tag" bug (see docs/CLI.md) before
    // it shipped, since Render() actually exercises markup parsing.
    private static string RenderToString()
    {
        var console = new TestConsole();
        var previousConsole = AnsiConsole.Console;
        AnsiConsole.Console = console;
        try
        {
            Banner.Render();
        }
        finally
        {
            AnsiConsole.Console = previousConsole;
        }

        return console.Output;
    }
}
