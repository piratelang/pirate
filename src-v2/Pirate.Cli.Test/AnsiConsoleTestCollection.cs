using Xunit;

namespace Pirate.Cli.Test;

/// <summary>
/// Tests that swap the shared static <c>AnsiConsole.Console</c> to capture
/// output via <c>Spectre.Console.Testing.TestConsole</c> must not run
/// concurrently with each other, or one test's swap-and-restore can clobber
/// another's — xUnit parallelizes across test classes by default, so every
/// such test class opts into this collection instead.
/// </summary>
[CollectionDefinition(nameof(AnsiConsoleTestCollection), DisableParallelization = true)]
public class AnsiConsoleTestCollection
{
}
