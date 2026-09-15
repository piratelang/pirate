namespace Pirate.Cli.Services;

/// <summary>
/// File templates used by "init" and "new", mirroring v1's inline templates
/// but updated for v2's grammar (docs/GRAMMAR.md) — a hello-world program
/// needs an explicit "extern" declaration and a typed "func main() : void".
/// </summary>
internal static class Templates
{
    public const string GitIgnore = "[Bb]in/";
    public const string GitAttributes = "*.pirate linguist-language=Squirrel";

    public static string HelloWorldPirate => string.Join(
        Environment.NewLine,
        "extern Standard.Terminal.Print;",
        "",
        "func main() : void",
        "{",
        "    Print(\"Hello World\");",
        "}"
    );
}
