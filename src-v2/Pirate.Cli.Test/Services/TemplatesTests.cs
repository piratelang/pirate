using Pirate.Cli.Services;
using Xunit;

namespace Pirate.Cli.Test.Services;

public class TemplatesTests
{
    [Fact]
    public void HelloWorldPirate_DeclaresPrintExternAndTypedMain()
    {
        var text = Templates.HelloWorldPirate;

        Assert.Contains("extern Standard.Terminal.Print;", text);
        Assert.Contains("func main() : void", text);
        Assert.Contains("Print(\"Hello World\");", text);
    }
}
