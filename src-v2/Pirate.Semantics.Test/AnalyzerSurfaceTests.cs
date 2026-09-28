using Pirate.Syntax;
using Pirate.Syntax.Nodes;

namespace Pirate.Semantics.Test;

/// <summary>
/// Coverage for the module-surface grammar: top-level statements, imports,
/// and exports (docs/GRAMMAR.md §3.1/§3.2).
/// </summary>
public class AnalyzerSurfaceTests
{
    [Fact]
    public void Analyze_TopLevelStatementProgram_Succeeds() =>
        Helpers.AssertClean("import standard Terminal; var name = \"World\"; PrintLine(name);");

    [Fact]
    public void Analyze_LeafCallWithoutImport_ReportsUndeclaredFunction() =>
        Helpers.AssertError(
            SemanticsErrorKind.UndeclaredFunction,
            "var greeting = \"hi\"; PrintLine(greeting);");

    [Fact]
    public void Analyze_ImportStandardBindsLeafNames_Succeeds() =>
        Helpers.AssertClean("import standard Terminal; PrintLine(\"ahoy\");");

    [Fact]
    public void Analyze_ImportStandardBindsDottedPaths_Succeeds() =>
        Helpers.AssertClean("import standard Terminal; Standard.Terminal.Print(\"ahoy\");");

    [Fact]
    public void Analyze_ImportStandard_ReadResultIsString()
    {
        var result = Helpers.Analyze("import standard Terminal; var input = Standard.Terminal.Read();");
        Assert.Empty(result.Errors);
        var declaration = result.Program.Statements.OfType<VariableDeclarationNode>().Single();
        Assert.NotNull(declaration.ResolvedSymbol);
        Assert.Equal(PirateType.String, declaration.ResolvedSymbol!.Type);
    }

    [Fact]
    public void Analyze_GlobalVariable_VisibleInsideFunction() =>
        Helpers.AssertClean("var shared = 10; func double() : int { return shared * 2; }");

    [Fact]
    public void Analyze_GlobalDeclaration_UsesGlobalScope()
    {
        var result = Helpers.Analyze("var shared = 10;");
        Assert.Empty(result.Errors);

        var declaration = result.Program.Statements.OfType<VariableDeclarationNode>().Single();
        Assert.Equal(SymbolScope.Global, declaration.ResolvedSymbol!.Scope);
    }

    [Theory]
    [InlineData("import standard IO;")] // no such builtin namespace
    [InlineData("import standard Nothing.At.All;")]
    public void Analyze_UnknownImport_ReportsUnknownImport(string source) =>
        Helpers.AssertError(SemanticsErrorKind.UnknownImport, source);

    [Theory]
    [InlineData("import module data as Data;")]
    [InlineData("import external shared as Shared;")]
    public void Analyze_ModuleOrExternalImport_ReportsUnsupported(string source) =>
        Helpers.AssertError(SemanticsErrorKind.ModuleImportUnsupported, source);

    [Theory]
    [InlineData("import module data as Data;", "Import of module 'data' is not supported yet — the module linker is a future milestone")]
    [InlineData("import external shared as Shared;", "Import of external 'shared' is not supported yet — the module linker is a future milestone")]
    public void Analyze_UnsupportedImport_MessageNamesTheActualKind(string source, string expectedMessage)
    {
        var error = Helpers.Analyze(source).Errors.Single();
        Assert.Equal(expectedMessage, error.Message);
    }

    [Fact]
    public void Analyze_ImportAndExternSamePath_ReportsDuplicateDeclaration() =>
        Helpers.AssertError(
            SemanticsErrorKind.DuplicateDeclaration,
            "extern Standard.Terminal.Print; import standard Terminal;");

    [Fact]
    public void Analyze_ImportLeafCollidesWithFunction_ReportsDuplicateDeclaration() =>
        Helpers.AssertError(
            SemanticsErrorKind.DuplicateDeclaration,
            "func PrintLine(string s) : void { } import standard Terminal;");

    [Fact]
    public void Analyze_ReturnAtTopLevel_ReportsReturnAtTopLevel() =>
        Helpers.AssertError(SemanticsErrorKind.ReturnAtTopLevel, "return;");

    [Fact]
    public void Analyze_ReturnWithExpressionAtTopLevel_ReportsReturnAtTopLevel() =>
        Helpers.AssertError(SemanticsErrorKind.ReturnAtTopLevel, "return 5;");

    [Fact]
    public void Analyze_ExportedVariableAndFunction_Succeed() =>
        Helpers.AssertClean("export var data = \"shared\"; export func get() : string { return data; }");

    [Fact]
    public void Analyze_ExportedConstStillImmutable() =>
        Helpers.AssertError(
            SemanticsErrorKind.AssignmentToConst,
            "export const LIMIT = 10; LIMIT = 11;");

    [Fact]
    public void Analyze_TopLevelControlFlowAndLoops_Succeed() =>
        Helpers.AssertClean(
            "var total = 0; " +
            "for var i = 0 to 3 { total = total + i; } " +
            "if total > 0 { var sign = \"positive\"; } " +
            "while total > 0 { total = total - 1; }");

    [Fact]
    public void Analyze_TopLevelIfWithoutElse_Succeeds() =>
        Helpers.AssertClean("var debug = true; if debug { var flag = 1; }");

    [Fact]
    public void Analyze_LambdalessForwardCallFromTopLevel_Succeeds() =>
        Helpers.AssertClean("import standard Terminal; PrintLine(makeGreeting()); func makeGreeting() : string { return \"ahoy\"; }");
}
