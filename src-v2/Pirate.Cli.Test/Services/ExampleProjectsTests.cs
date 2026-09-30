using Pirate.Cli.Services;
using Pirate.Syntax;
using Xunit;

namespace Pirate.Cli.Test.Services;

/// <summary>
/// The programs under docs/examples are the canonical grammar usage (see
/// docs/examples/README.md); these tests pin them against the real
/// frontend, so grammar and examples can never silently drift apart again.
/// </summary>
public class ExampleProjectsTests
{
    private static readonly string ExamplesRoot = FindExamplesRoot();

    private static string FindExamplesRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, "docs", "examples");
            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate docs/examples above the test output directory.");
    }

    private static FrontendResult CompileExample(string folder, string file) =>
        Pipelines.Compile(File.ReadAllText(Path.Combine(ExamplesRoot, folder, file)));

    [Theory]
    [InlineData("1 - Hello World", "main.pirate")]
    [InlineData("2 - More Operations", "main.pirate")]
    [InlineData("4 - Multi Module", "data.pirate")] // the helper module is linker-free to check
    public void CompileExample_ChecksCleanThroughFrontend(string folder, string file)
    {
        var result = CompileExample(folder, file);

        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData("4 - Multi Module", "main.pirate")]
    [InlineData("5 - External Module", "main.pirate")]
    public void CompileExample_SpecExample_ReportsModuleImportUnsupported(string folder, string file)
    {
        var result = CompileExample(folder, file);

        // SEM-013 until the linker lands; references to the un-linked alias
        // (Data.data) additionally report undeclared names — the spec
        // examples are honest about being ahead of the tooling.
        Assert.Contains(result.Errors, e => e is SemanticsError { Kind: SemanticsErrorKind.ModuleImportUnsupported });
    }

    [Theory]
    [InlineData("1 - Hello World")]
    [InlineData("2 - More Operations")]
    [InlineData("4 - Multi Module")]
    [InlineData("5 - External Module")]
    public void EveryExample_ManifestRoundTripsThroughFleetModel(string folder)
    {
        // Read through the real manifest model, not raw JSON: an example
        // whose manifest the tooling itself cannot load is not an example.
        var fleet = Pirate.Fleet.FleetFileRepository.TryRead(Path.Combine(ExamplesRoot, folder));

        Assert.NotNull(fleet);
        Assert.Equal("main", fleet!.EntryPoint);
        Assert.False(string.IsNullOrEmpty(fleet.Name));
        Assert.False(string.IsNullOrEmpty(fleet.Version));
    }
}
