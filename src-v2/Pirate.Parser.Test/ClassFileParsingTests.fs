namespace Pirate.Parser.Test

open Pirate.Parser
open Pirate.Syntax
open Pirate.Syntax.Nodes
open Xunit

/// Per-production tests for the class-file grammar (docs/GRAMMAR.md §4,
/// flat files Phase 2). No loose statements — every class file is parsed
/// with <c>PirateFileKind.Class</c>, which only 'field'/'const'/
/// 'constructor'/methods produce members for.
module ClassFileParsingTests =

    [<Fact>]
    let ``Typed field`` () =
        let result = Helpers.parseClass "field int count = 0;"
        Assert.Empty(result.Errors)
        let field = result.Program.Value.Members.[0] :?> FieldDeclarationNode
        Assert.Equal("count", field.Name)
        Assert.Equal(ScalarType.Int, field.Type.ScalarType)
        Assert.False(field.IsConst)
        Assert.False(field.IsPrivate)
        Assert.False(field.IsReadonly)
        Assert.NotNull(field.Initializer)

    [<Fact>]
    let ``Field with var infers type`` () =
        let result = Helpers.parseClass "field var label = \"x\";"
        Assert.Empty(result.Errors)
        let field = result.Program.Value.Members.[0] :?> FieldDeclarationNode
        Assert.Null(field.Type)
        Assert.Equal("label", field.Name)

    [<Fact>]
    let ``Field with no initializer is legal syntax`` () =
        // Definite assignment (every constructor must assign it) is a
        // semantics concern (Phase 4), not the parser's.
        let result = Helpers.parseClass "field int count;"
        Assert.Empty(result.Errors)
        let field = result.Program.Value.Members.[0] :?> FieldDeclarationNode
        Assert.Null(field.Initializer)

    [<Fact>]
    let ``Readonly and private modifiers`` () =
        let result = Helpers.parseClass "readonly field int count = 0;\nprivate field int calls = 0;"
        Assert.Empty(result.Errors)
        let readonlyField = result.Program.Value.Members.[0] :?> FieldDeclarationNode
        let privateField = result.Program.Value.Members.[1] :?> FieldDeclarationNode
        Assert.True(readonlyField.IsReadonly)
        Assert.False(readonlyField.IsPrivate)
        Assert.True(privateField.IsPrivate)
        Assert.False(privateField.IsReadonly)

    [<Fact>]
    let ``Nullable field type`` () =
        let result = Helpers.parseClass "field int? count = null;"
        Assert.Empty(result.Errors)
        let field = result.Program.Value.Members.[0] :?> FieldDeclarationNode
        Assert.True(field.Type.IsNullable)
        let init = field.Initializer :?> LiteralNode
        Assert.Equal(LiteralKind.Null, init.LiteralKind)

    [<Fact>]
    let ``Class-name field type is accepted, unresolved`` () =
        let result = Helpers.parseClass "field Money price = new Money(0);"
        Assert.Empty(result.Errors)
        let field = result.Program.Value.Members.[0] :?> FieldDeclarationNode
        Assert.Equal("Money", field.Type.ClassName)
        let init = field.Initializer :?> NewExpressionNode
        Assert.Equal("Money", init.ClassName)
        Assert.Single(init.Arguments)

    [<Fact>]
    let ``Class constant without field keyword`` () =
        let result = Helpers.parseClass "const int Limit = 1;"
        Assert.Empty(result.Errors)
        let field = result.Program.Value.Members.[0] :?> FieldDeclarationNode
        Assert.True(field.IsConst)
        Assert.Equal("Limit", field.Name)

    [<Fact>]
    let ``Const with no initializer is a syntax error`` () =
        let result = Helpers.parseClass "const int Limit;"
        Assert.True(
            result.Errors
            |> Seq.exists (fun e ->
                match e with
                | :? SyntaxError as s -> s.Kind = SyntaxErrorKind.MissingEqualsInDeclaration
                | _ -> false))

    [<Fact>]
    let ``Constructor with no delegate`` () =
        let result = Helpers.parseClass "constructor() { }"
        Assert.Empty(result.Errors)
        let ctor = result.Program.Value.Members.[0] :?> ConstructorDeclarationNode
        Assert.Empty(ctor.Parameters)
        Assert.Null(ctor.DelegateArguments)

    [<Fact>]
    let ``Constructor with parameters`` () =
        let result = Helpers.parseClass "constructor(int start) { count = start; }"
        Assert.Empty(result.Errors)
        let ctor = result.Program.Value.Members.[0] :?> ConstructorDeclarationNode
        Assert.Single(ctor.Parameters)
        Assert.Equal("start", ctor.Parameters.[0].Name)

    [<Fact>]
    let ``Constructor delegating to self`` () =
        let result = Helpers.parseClass "constructor() : self(16) { }"
        Assert.Empty(result.Errors)
        let ctor = result.Program.Value.Members.[0] :?> ConstructorDeclarationNode
        Assert.NotNull(ctor.DelegateArguments)
        Assert.Single(ctor.DelegateArguments)

    [<Fact>]
    let ``Method in a class file`` () =
        let result = Helpers.parseClass "func value() : int { return count; }"
        Assert.Empty(result.Errors)
        let method' = result.Program.Value.Members.[0] :?> FunctionDeclarationNode
        Assert.Equal("value", method'.Name)

    [<Fact>]
    let ``Private method`` () =
        let result = Helpers.parseClass "private func reset() : void { count = 0; }"
        Assert.Empty(result.Errors)
        let method' = result.Program.Value.Members.[0] :?> FunctionDeclarationNode
        Assert.True(method'.IsPrivate)

    [<Fact>]
    let ``self as an expression`` () =
        let result = Helpers.parseClass "constructor(int cents) { self.cents = cents; }"
        Assert.Empty(result.Errors)
        let ctor = result.Program.Value.Members.[0] :?> ConstructorDeclarationNode
        let assignment = ctor.Body.Statements.[0] :?> MemberAssignmentNode
        Assert.Equal("cents", assignment.Member)
        Assert.IsType<SelfExpressionNode>(assignment.Target) |> ignore

    [<Fact>]
    let ``Loose statement in a class file is a syntax error`` () =
        let result = Helpers.parseClass "var x = 5;"
        Assert.True(
            result.Errors
            |> Seq.exists (fun e ->
                match e with
                | :? SyntaxError as s -> s.Kind = SyntaxErrorKind.MissingFieldOrMemberInClassFile
                | _ -> false))

    [<Theory>]
    [<InlineData("extends Foo;")>]
    [<InlineData("implements Foo;")>]
    [<InlineData("static field int x = 0;")>]
    [<InlineData("abstract func f() : void;")>]
    [<InlineData("override func f() : void { }")>]
    let ``Reserved keywords are rejected as not supported yet`` (source: string) =
        let result = Helpers.parseClass source
        Assert.True(
            result.Errors
            |> Seq.exists (fun e ->
                match e with
                | :? SyntaxError as s -> s.Kind = SyntaxErrorKind.ReservedKeywordNotSupportedYet
                | _ -> false))

    [<Fact>]
    let ``Constructor delegating to super is reserved, not supported yet`` () =
        let result = Helpers.parseClass "constructor() : super() { }"
        Assert.True(
            result.Errors
            |> Seq.exists (fun e ->
                match e with
                | :? SyntaxError as s -> s.Kind = SyntaxErrorKind.ReservedKeywordNotSupportedYet
                | _ -> false))

    [<Fact>]
    let ``Counter example parses clean`` () =
        // docs/examples/6 - Classes/Counter.cpirate
        let source = """
const int Step = 1;

readonly field int count = 0;
private field int calls = 0;

constructor() { }

constructor(int start) {
    count = start;
}

func increment() : void {
    count = count + Step;
    calls = calls + 1;
}

func value() : int {
    return count;
}

private func reset() : void {
    count = 0;
}
"""
        let result = Helpers.parseClass source
        Assert.Empty(result.Errors)
        // const Step, readonly field count, private field calls, two
        // constructors, increment, value, private reset.
        Assert.Equal(8, result.Program.Value.Members.Count)
