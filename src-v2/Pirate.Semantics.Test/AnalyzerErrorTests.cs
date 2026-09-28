using Pirate.Syntax;

namespace Pirate.Semantics.Test;

public class AnalyzerErrorTests
{
    // --- SEM-001: undeclared variable (read AND assignment both) ---

    [Theory]
    [InlineData("var y = x;")]
    [InlineData("x = 5;")]
    public void Analyze_UndeclaredVariableReadOrAssign_ReportsUndeclaredVariable(string body) =>
        Helpers.AssertErrorInMain(SemanticsErrorKind.UndeclaredVariable, body);

    [Fact]
    public void Analyze_UndeclaredArgumentToKnownBuiltin_ReportsUndeclaredVariable() =>
        Helpers.AssertError(
            SemanticsErrorKind.UndeclaredVariable,
            "extern Standard.Terminal.Print; func main() : void { Standard.Terminal.Print(x); }");

    [Fact]
    public void Analyze_NameDeclaredInOtherFunction_ReportsUndeclaredVariable() =>
        Helpers.AssertError(
            SemanticsErrorKind.UndeclaredVariable,
            "func f() : void { var z = 1; } func main() : void { var y = z; }");

    // --- SEM-002: undeclared function ---

    [Theory]
    [InlineData("func main() : void { nope(); }")]
    [InlineData("func main() : void { Standard.Terminal.Print(\"x\"); }")] // no extern imported
    public void UndeclaredFunction(string source) =>
        Helpers.AssertError(SemanticsErrorKind.UndeclaredFunction, source);

    // --- SEM-003: type mismatch family ---

    [Theory]
    [InlineData("int x = \"hello\";")]
    [InlineData("float f = 1;")] // no implicit widening
    [InlineData("var a = 1; var b = 1.5; var c = a + b;")] // mixed numeric
    [InlineData("var z = 1 + \"x\";")]
    [InlineData("var z = true + 1;")]
    [InlineData("var z = \"a\" * \"b\";")]
    [InlineData("var z = !5;")]
    [InlineData("var z = -true;")]
    [InlineData("var z = -\"s\";")]
    [InlineData("var z = true < false;")] // relational on bool
    [InlineData("char a = 'a'; if a < 'b' { var y = 1; }")] // char ordering deferred
    [InlineData("if \"a\" < \"b\" { var y = 1; }")] // string ordering deferred
    [InlineData("int[] a1 = [1]; int[] a2 = [2]; bool e = a1 == a2;")] // array equality deferred
    [InlineData("var z = [1, \"a\"];")] // heterogeneous literal
    [InlineData("int[] n = [\"a\"];")]
    [InlineData("var s = \"abc\"; var c = s[0];")] // string indexing deferred
    [InlineData("int x = 1; var y = x[0];")] // index non-array
    [InlineData("int[] xs = [1]; var y = xs[\"a\"];")] // non-int index
    [InlineData("if 5 { var x = 1; }")] // non-bool if condition
    [InlineData("while 1 { var x = 1; }")] // non-bool while condition
    [InlineData("var n = \"s\"; for var i = 0 to n { }")] // non-int loop bound
    [InlineData("1 + 2;")] // expression statement is not a call
    public void Analyze_TypeErrorsInMain_ReportsTypeMismatch(string body) =>
        Helpers.AssertErrorInMain(SemanticsErrorKind.TypeMismatch, body);

    [Theory]
    [InlineData("func f(int a) : void { } func main() : void { f(\"x\"); }")] // argument type
    [InlineData("func f(int a) : void { } func main() : void { f(1, 2); }")] // argument count
    [InlineData("func f() : void { } func main() : void { var v = f(); }")] // void used as value
    [InlineData("func f() : int { return 1; } func main() : void { var g = f; }")] // function as value
    [InlineData("func main() : void { var p = 1; p(2); }")] // variable called
    public void Analyze_TypeErrorsAcrossDeclarations_ReportsTypeMismatch(string source) =>
        Helpers.AssertError(SemanticsErrorKind.TypeMismatch, source);

    [Theory]
    [InlineData("func f(void x) : void { }")] // void parameter type
    [InlineData("func f() : void[] { return; }")] // void[] return type
    public void Analyze_VoidTypePosition_ReportsTypeMismatch(string source) =>
        Helpers.AssertError(SemanticsErrorKind.TypeMismatch, source);

    [Fact]
    public void Analyze_AssignToFunctionName_ReportsTypeMismatch() =>
        Helpers.AssertError(SemanticsErrorKind.TypeMismatch, "func main() : void { main = 5; }");

    [Fact]
    public void Analyze_WrongInitializerType_MessageMatchesCatalogFormat()
    {
        var error = Helpers.Analyze("func main() : void { int x = \"hello\"; }").Errors.Single();
        Assert.Equal("Type mismatch: expected 'int', got 'string'", error.Message);
    }

    // --- SEM-004: assign to const ---

    [Theory]
    [InlineData("const int x = 5; x = 6;")]
    [InlineData("const var g = \"a\"; g = \"b\";")]
    [InlineData("const int[] a = [1]; a[0] = 2;")] // element writes are blocked too
    public void Analyze_AssignToConst_ReportsAssignmentToConst(string body) =>
        Helpers.AssertErrorInMain(SemanticsErrorKind.AssignmentToConst, body);

    [Fact]
    public void Analyze_AssignToConst_PointsAtAssignment()
    {
        var error = Helpers.Analyze("func main() : void { const int x = 5; x = 6; }").Errors.Single();
        Assert.Equal(1, error.StartLocation.Line);
        Assert.Equal(39, error.StartLocation.Column);
    }

    // --- SEM-005: missing return ---

    [Theory]
    [InlineData("func f() : int { }")]
    [InlineData("func f(bool b) : int { if b { return 1; } }")] // if without else
    [InlineData("func f(bool b) : int { if b { return 1; } else { if b { return 2; } } }")] // inner if lacks else
    public void Analyze_UncoveredReturnPaths_ReportsMissingReturn(string source) =>
        Helpers.AssertError(SemanticsErrorKind.MissingReturnInNonVoidFunction, source);

    // --- SEM-006 / SEM-007: return shape ---

    [Fact]
    public void Analyze_ReturnWithValueInVoid_ReportsVoidFunctionReturnsValue() =>
        Helpers.AssertError(SemanticsErrorKind.VoidFunctionReturnsValue, "func main() : void { return 42; }");

    [Theory]
    [InlineData("func f() : int { return; }")]
    [InlineData("func f(bool b) : int { if b { return 1; } return; }")]
    public void Analyze_BareReturnInNonVoid_ReportsReturnValueRequired(string source) =>
        Helpers.AssertError(SemanticsErrorKind.ReturnValueRequired, source);

    // --- SEM-008: empty array needs an element type ---

    [Fact]
    public void Analyze_EmptyArrayWithVar_ReportsEmptyArrayRequiresElementType() =>
        Helpers.AssertErrorInMain(SemanticsErrorKind.EmptyArrayRequiresElementType, "var list = [];");

    [Fact]
    public void Analyze_EmptyArrayWithExplicitType_Succeeds() =>
        Helpers.AssertCleanInMain("int[] list = [];");

    // --- SEM-009: for-in iterable ---

    [Fact]
    public void Analyze_ForInOverScalar_ReportsForInIterableMustBeArray() =>
        Helpers.AssertErrorInMain(SemanticsErrorKind.ForInIterableMustBeArray, "var n = 5; for (item in n) { }");

    // --- SEM-010: duplicates ---

    [Theory]
    [InlineData("func main() : void { var x = 1; var x = 2; }")]
    [InlineData("func main() : void { var x = 1; int x = 2; }")]
    [InlineData("func main() : void { var x = 1; for var x = 0 to 1 { } }")]
    [InlineData("func f(int a, int a) : void { }")]
    [InlineData("func f() : void { } func f() : void { }")]
    [InlineData("extern Standard.Terminal.Print; extern Standard.Terminal.Print;")]
    public void Analyze_RedeclaredNames_ReportsDuplicateDeclaration(string source) =>
        Helpers.AssertError(SemanticsErrorKind.DuplicateDeclaration, source);

    [Fact]
    public void Analyze_DuplicateNonVoidFunction_ReportsNoBodyCascade()
    {
        // The duplicate reports once (SEM-010); the second copy must not be
        // re-checked against the first declaration's symbol, so the broken
        // body's missing-return surfaces exactly once — from the original.
        var result = Helpers.Analyze("func f() : int { } func f() : int { }");

        Assert.Equal(2, result.Errors.Count);
        Assert.Single(result.Errors, e => e.Kind == SemanticsErrorKind.DuplicateDeclaration);
        Assert.Single(result.Errors, e => e.Kind == SemanticsErrorKind.MissingReturnInNonVoidFunction);
    }

    [Fact]
    public void Analyze_ImportCollision_BindsNothingFromTheGroup()
    {
        // Atomic import: the colliding leaf (PrintLine) rejects the whole
        // group, so Print — which would have bound cleanly — is undeclared.
        var result = Helpers.Analyze(
            "func PrintLine(string s) : void { } import standard Terminal; Print(\"x\");");

        Assert.Contains(result.Errors, e => e.Kind == SemanticsErrorKind.DuplicateDeclaration);
        Assert.Contains(result.Errors, e => e.Kind == SemanticsErrorKind.UndeclaredFunction);
    }

    // --- SEM-011: unknown extern ---

    [Theory]
    [InlineData("extern Standard.Foo.Bar; func main() : void { }")]
    [InlineData("extern Standard.String.Nope; func main() : void { }")]
    [InlineData("extern Print; func main() : void { }")]
    public void Analyze_UnknownExternPath_ReportsUnknownExtern(string source) =>
        Helpers.AssertError(SemanticsErrorKind.UnknownExtern, source);

    [Fact]
    public void Analyze_UnusedKnownExtern_Succeeds() =>
        Helpers.AssertClean("extern Standard.Terminal.Print; func main() : void { }");

    // --- no false positives: the entry-point rule is NOT semantic ---

    [Fact]
    public void ProgramWithoutMain_IsSemanticallyClean() =>
        Helpers.AssertClean("func helper() : int { return 1; }");

    [Fact]
    public void Analyze_ForInOverUndeclaredName_ReportsUndeclaredVariableOnly()
    {
        var result = Helpers.Analyze("func main() : void { for (item in nothing) { } }");
        var error = Assert.Single(result.Errors);
        Assert.Equal(SemanticsErrorKind.UndeclaredVariable, error.Kind);
    }
}
