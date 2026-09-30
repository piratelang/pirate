using Pirate.Cli.Services;
using Xunit;

namespace Pirate.Cli.Test.Services;

public class TemplatesTests
{
    [Fact]
    public void HelloWorldPirate_ImportsTerminalAndCallsPrintLineAtTopLevel()
    {
        var text = Templates.HelloWorldPirate;

        Assert.Contains("import standard Terminal;", text);
        Assert.Contains("PrintLine(\"Hello World\");", text);
    }
}
