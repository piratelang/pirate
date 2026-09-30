using Pirate.Cli.Services;
using Pirate.Semantics;
using Pirate.Syntax;
using Pirate.Syntax.Nodes;

namespace Pirate.Cli.Test.Services;

// The pipeline is internal to Pirate.Cli (InternalsVisibleTo); tests
// construct the implementation directly, per STYLE.md testing rules.
internal static class Pipelines
{
    public static FrontendResult Compile(string source) =>
        new CompilationPipeline(new SemanticAnalyzer(), new Pirate.Lexer.Lexer(), new Pirate.Parser.ParserService())
            .Compile(source);
}

public class CompilationPipelineTests
{

    [Fact]
    public void CleanProgram_SucceedsAndReturnsCheckedProgram()
    {
        var result = Pipelines.Compile("func main() : void { var x = 5; }");

        Assert.True(result.Success);
        Assert.NotNull(result.Program);
    }

    [Fact]
    public void SemanticError_FailsPipeline_AndIsTheOnlyError()
    {
        var result = Pipelines.Compile("func main() : void { int x = \"s\"; }");

        Assert.False(result.Success);
        Assert.Single(result.Errors, e => e is SemanticsError);
    }

    [Fact]
    public void SyntaxError_SkipsSemantics_NoCascadeOfTypeErrors()
    {
        // `var x = ;` breaks the parse inside main; a semantic run over the
        // partial AST would double-report — so semantics must not run.
        var result = Pipelines.Compile("func main() : void { var x = ; int y = \"text\"; }");

        Assert.NotEmpty(result.Errors.OfType<SyntaxError>());
        Assert.Empty(result.Errors.OfType<SemanticsError>());
    }

    [Fact]
    public void ExternAndCall_ResolvesThroughSemantics()
    {
        var result = Pipelines.Compile(
            "extern Standard.Terminal.Print; func main() : void { Standard.Terminal.Print(\"hi\"); }");

        Assert.True(result.Success);
    }

    [Fact]
    public void Compile_MultipleErrors_IsOrderedBySourcePosition()
    {
        // Pass order (imports -> top-level -> bodies) differs from source
        // order here; the rendered list must be top-down by position.
        var result = Pipelines.Compile("func f() : int { }\nint x = \"s\";\nx = \"t\";");

        var lines = result.Errors.Select(error => error.StartLocation.Line).ToList();

        Assert.Equal(3, lines.Count);
        Assert.Equal(lines.OrderBy(line => line).ToList(), lines);
    }

    [Fact]
    public void UnknownExtern_ReachesErrorMapperAsSem011()
    {
        var result = Pipelines.Compile("extern Standard.Nope.Thing; func main() : void { }");

        var error = Assert.Single(result.Errors, e => e is SemanticsError);
        Assert.Equal("SEM-011", ErrorMapper.Map(error));
    }

    [Fact]
    public void ShippedHelloWorldTemplate_TypeChecksClean()
    {
        // Guards against the template drifting from the grammar again: a
        // fresh `pirate new` project must survive the whole frontend.
        var result = Pipelines.Compile(Templates.HelloWorldPirate);

        Assert.Empty(result.Errors);
        Assert.True(EntryPoint.HasRunnableBody(result.Program));
    }

    [Fact]
    public void TopLevelImportAndCall_TypeCheckClean()
    {
        var result = Pipelines.Compile(
            "import standard Terminal; var name = \"World\"; PrintLine(name);");

        Assert.Empty(result.Errors);
    }
}

public class EntryPointTests
{
    private static ProgramNode? ProgramOf(string source) =>
        Pipelines.Compile(source).Program;

    [Theory]
    [InlineData("import standard Terminal; PrintLine(\"hello\");")] // a bare top-level call is a runnable program
    [InlineData("var x = 5;")] // declarations count as top-level code too
    [InlineData("import standard Terminal; func main() : void { } PrintLine(\"hi\");")] // helpers alongside the body
    public void HasRunnableBody_TopLevelStatements(string source) =>
        Assert.True(EntryPoint.HasRunnableBody(ProgramOf(source)));

    [Theory]
    [InlineData("func helper() : void { }")] // declarations only
    [InlineData("import standard Terminal;")] // imports only
    [InlineData("extern Standard.Terminal.Print;")]
    [InlineData("")] // nothing at all
    public void HasRunnableBody_DeclarationOnlyModule(string source) =>
        Assert.False(EntryPoint.HasRunnableBody(ProgramOf(source)));

    [Fact]
    public void HasRunnableBody_NullProgram_IsFalse() =>
        Assert.False(EntryPoint.HasRunnableBody(null));
}
