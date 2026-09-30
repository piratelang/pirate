using Pirate.Syntax;

namespace Pirate.Semantics.Test;

/// <summary>
/// Source-driven helper: runs the real lexer and parser, then the analyzer,
/// so every test exercises the same pipeline the CLI will. Lexer/Parser are
/// fully qualified because inside a Pirate.* namespace the simple names bind
/// to the namespaces, not the classes.
/// </summary>
internal static class Helpers
{
    public static SemanticResult Analyze(string source)
    {
        var lex = new Pirate.Lexer.Lexer().Tokenize(source);
        Assert.Empty(lex.Errors);

        var parse = Pirate.Parser.Parser.Parse(lex, PirateFileKind.Module);
        Assert.Empty(parse.Errors);
        Assert.NotNull(parse.Program);

        return new SemanticAnalyzer().Analyze(parse.Program.Value);
    }

    public static void AssertClean(string source)
    {
        var result = Analyze(source);
        Assert.Empty(result.Errors);
    }

    public static void AssertError(SemanticsErrorKind kind, string source)
    {
        var errors = Analyze(source).Errors;
        var match = Assert.Single(errors, e => e.Kind == kind);
        Assert.False(string.IsNullOrEmpty(match.Message));
    }

    public static void AssertNoErrorKind(SemanticsErrorKind kind, string source) =>
        Assert.DoesNotContain(Analyze(source).Errors, e => e.Kind == kind);

    /// <summary>Body wrapped in a void main for statement-level tests.</summary>
    public static void AssertCleanInMain(string body) =>
        AssertClean($"func main() : void {{ {body} }}");

    public static void AssertErrorInMain(SemanticsErrorKind kind, string body) =>
        AssertError(kind, $"func main() : void {{ {body} }}");
}
