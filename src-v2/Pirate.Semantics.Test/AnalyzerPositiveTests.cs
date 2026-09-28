using Pirate.Syntax;
using Pirate.Syntax.Nodes;

namespace Pirate.Semantics.Test;

public class AnalyzerPositiveTests
{
    [Fact]
    public void Analyze_VarReassignmentSameType_Succeeds() =>
        Helpers.AssertClean("func main() : void { var x = 5; x = 6; }");

    [Fact]
    public void Analyze_AllScalarDeclarations_Succeeds() =>
        Helpers.AssertClean(
            "func main() : void { " +
            "int x = 5; float f = 1.5; string s = \"ahoy\"; char c = 'x'; bool b = true; }");

    [Theory]
    [InlineData("var x = 5;", "int")]
    [InlineData("var x = 1.5;", "float")]
    [InlineData("var x = \"s\";", "string")]
    [InlineData("var x = 'c';", "char")]
    [InlineData("var x = true;", "bool")]
    [InlineData("var x = [1, 2];", "int[]")]
    [InlineData("var x = [\"a\"];", "string[]")]
    [InlineData("var x = 1 + 2;", "int")]
    [InlineData("var x = \"a\" + \"b\";", "string")]
    [InlineData("var x = 1 == 2;", "bool")]
    [InlineData("var x = 1 < 2;", "bool")]
    [InlineData("var x = true && false;", "bool")]
    [InlineData("var x = !true;", "bool")]
    [InlineData("var x = -5;", "int")]
    [InlineData("var x = 2 ^ 3;", "int")]
    [InlineData("var x = 7 % 2;", "int")]
    public void Analyze_InferredLiteralAndOperationTypes_AnnotatedOnExpression(string statement, string expectedType)
    {
        var result = Helpers.Analyze($"func main() : void {{ {statement} }}");
        Assert.Empty(result.Errors);

        var declaration = GetMain(result).Body.Statements.OfType<VariableDeclarationNode>().Single();
        Assert.NotNull(declaration.Initializer.InferredType);
        Assert.Equal(expectedType, declaration.Initializer.InferredType.ToString());
    }

    [Fact]
    public void Analyze_DottedExternCallStatement_Succeeds() =>
        Helpers.AssertClean(
            "extern Standard.Terminal.Print; func main() : void { Standard.Terminal.Print(\"yo\"); }");

    [Fact]
    public void Analyze_IndexedCallResult_Succeeds() =>
        Helpers.AssertClean(
            "extern Standard.String.Split; func main() : void { " +
            "string part = Standard.String.Split(\"a,b\", \",\")[0]; }");

    [Fact]
    public void Analyze_ExternResultsInValuePositions_Succeeds() =>
        Helpers.AssertClean(
            "extern Standard.Terminal.Read; " +
            "extern Standard.String.Length; " +
            "func main() : void { var s = Standard.Terminal.Read(); int len = Standard.String.Length(s); }");

    [Fact]
    public void Analyze_RecursionAndForwardReferences_Succeeds() =>
        Helpers.AssertClean(
            "func a() : int { return b(); } " +
            "func b() : int { return 5; } " +
            "func main() : void { var x = a(); }");

    [Fact]
    public void Analyze_FibonacciProgram_Succeeds() =>
        Helpers.AssertClean(
            "func fib(int n) : int { if n < 2 { return n; } return fib(n - 1) + fib(n - 2); } " +
            "func main() : void { var x = fib(10); }");

    [Fact]
    public void Analyze_IfElseChainReturn_Succeeds() =>
        Helpers.AssertClean(
            "func label(int x) : string { " +
            "if x == 1 { return \"one\"; } " +
            "else if x == 2 { return \"two\"; } " +
            "else { return \"many\"; } }");

    [Fact]
    public void Analyze_TrailingReturnInNonVoid_Succeeds() =>
        Helpers.AssertClean("func f() : int { return 42; }");

    [Fact]
    public void Analyze_BareReturnInVoid_Succeeds() =>
        Helpers.AssertClean("func main() : void { return; }");

    [Fact]
    public void Analyze_ValueCallAsStatement_Succeeds() =>
        Helpers.AssertClean("func f() : int { return 1; } func main() : void { f(); }");

    [Fact]
    public void Analyze_NumericOperatorTable_Succeeds() =>
        Helpers.AssertClean(
            "func main() : void { " +
            "int a = 1 + 2 - 3 * 4 / 5 % 6 ^ 7; " +
            "float b = 1.5 + 2.5 - 3.5 * 4.5 / 5.5; }");

    [Fact]
    public void Analyze_ComparisonAndLogic_Succeeds() =>
        Helpers.AssertClean(
            "func main() : void { " +
            "bool x = 1 < 2 && 2.5 <= 3.5 || !(4 == 5) && \"a\" != \"b\" && 'c' == 'c'; }");

    [Fact]
    public void Analyze_ArrayDeclarationLiteralIndexAssign_Succeeds() =>
        Helpers.AssertClean(
            "func main() : void { int[] xs = [1, 2, 3]; int[] e = []; xs[0] = 9; var v = xs[1]; }");

    [Fact]
    public void Analyze_CountingForLoop_Succeeds() =>
        Helpers.AssertClean(
            "func main() : void { for var i = 0 to 10 { var j = i * 2; } }");

    [Fact]
    public void Analyze_ForInLoop_Succeeds() =>
        Helpers.AssertClean(
            "func main() : void { string[] names = [\"a\", \"b\"]; " +
            "for (n in names) { var m = n + \"!\"; n = m; } }");

    [Fact]
    public void Analyze_BoolWhileCondition_Succeeds() =>
        Helpers.AssertClean("func main() : void { bool go = true; while go { go = false; } }");

    [Fact]
    public void Analyze_ConstDeclarationAndReads_Succeeds() =>
        Helpers.AssertClean("func main() : void { const int limit = 2 + 3; var y = limit * 2; }");

    [Fact]
    public void Analyze_LocalShadowsGlobalFunction_Succeeds() =>
        Helpers.AssertClean(
            "func f() : int { return 9; } " +
            "func main() : void { int f = 5; var y = f + 1; }");

    [Fact]
    public void Analyze_DeclarationsInFunction_AssignSequentialSlotIndexes()
    {
        var result = Helpers.Analyze(
            "extern Standard.Terminal.Print; " +
            "extern Standard.String.Length; " +
            "func f(int p1, int p2) : void { var v = 1; for var i = 0 to 1 { var w = 2; } int n = Standard.String.Length(\"abc\"); }");
        Assert.Empty(result.Errors);

        var function = result.Program.Members.OfType<FunctionDeclarationNode>().Single(m => m.Name == "f");
        Assert.Equal(0, function.Parameters[0].ResolvedSymbol!.Index);
        Assert.Equal(1, function.Parameters[1].ResolvedSymbol!.Index);

        var declaration = function.Body.Statements.OfType<VariableDeclarationNode>().Single(d => d.Name == "v");
        Assert.Equal(2, declaration.ResolvedSymbol!.Index);

        var forStatement = function.Body.Statements.OfType<ForStatementNode>().Single();
        Assert.Equal(3, forStatement.LoopVariable!.Index);
        var inner = forStatement.Body.Statements.OfType<VariableDeclarationNode>().Single();
        Assert.Equal(4, inner.ResolvedSymbol!.Index);

        // Extern-resolved calls carry the builtin registry index.
        var lengthCall = function.Body.Statements
            .OfType<VariableDeclarationNode>()
            .Single(d => d.Name == "n")
            .Initializer as FunctionCallNode;
        Assert.Equal(3, Assert.IsType<BuiltinSymbol>(lengthCall!.ResolvedCallee).Index);
    }

    [Fact]
    public void Analyze_DeclarationsAndCalls_AnnotateResolvedSymbols()
    {
        var result = Helpers.Analyze(
            "extern Standard.Terminal.Print; func main() : void { var x = 5; Print_placeholder_guard(); } " +
            "func Print_placeholder_guard() : void { Standard.Terminal.Print(\"ok\"); }");
        Assert.Empty(result.Errors);

        var main = GetMain(result);
        var declaration = main.Body.Statements.OfType<VariableDeclarationNode>().Single();
        Assert.Equal(SymbolScope.Local, declaration.ResolvedSymbol!.Scope);
        Assert.Equal(PirateType.Int, declaration.ResolvedSymbol!.Type);

        var call = main.Body.Statements.OfType<ExpressionStatementNode>().Single().Expression as FunctionCallNode;
        Assert.NotNull(call);
        Assert.IsType<FunctionSymbol>(call!.ResolvedCallee);
    }

    [Fact]
    public void Analyze_BuiltinCall_AnnotatesCalleeSymbol()
    {
        var result = Helpers.Analyze(
            "extern Standard.Terminal.Print; func main() : void { Standard.Terminal.Print(\"hi\"); }");
        Assert.Empty(result.Errors);

        var call = GetMain(result).Body.Statements.OfType<ExpressionStatementNode>().Single().Expression as FunctionCallNode;
        var builtin = Assert.IsType<BuiltinSymbol>(call!.ResolvedCallee);
        Assert.Equal("Standard.Terminal.Print", builtin.Name);
        Assert.Equal(SymbolScope.Builtin, builtin.Scope);
    }

    private static FunctionDeclarationNode GetMain(SemanticResult result) =>
        result.Program.Members.OfType<FunctionDeclarationNode>().Single(f => f.Name == "main");
}
