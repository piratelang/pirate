namespace Pirate.Cli.Services;

/// <summary>
/// File templates used by "init" and "new". The hello-world program follows
/// docs/GRAMMAR.md: an <c>import standard</c> binding of the Terminal group
/// and a top-level call in the entry module.
/// </summary>
internal static class Templates
{
    public const string GitIgnore = "[Bb]in/";
    public const string GitAttributes = "*.pirate linguist-language=Squirrel";

    public static string HelloWorldPirate => string.Join(
        Environment.NewLine,
        "import standard Terminal;",
        "",
        "PrintLine(\"Hello World\");"
    );
}
