namespace Pirate.Parser.Test

open Pirate.Parser
open Pirate.Syntax
open Pirate.Syntax.Nodes
open Xunit

module ModuleParsingTests =

    [<Fact>]
    let ``Top-level statements are collected separately from members`` () =
        let result = Helpers.parse "var x = 5; func helper() : void { } x = 6;"
        Assert.Empty(result.Errors)
        let program = result.Program.Value
        Assert.Single(program.Members)
        Assert.IsType<FunctionDeclarationNode>(program.Members.[0])
        Assert.Equal(2, program.Statements.Count)
        Assert.IsType<VariableDeclarationNode>(program.Statements.[0])
        Assert.IsType<VariableAssignmentNode>(program.Statements.[1])

    [<Fact>]
    let ``Statement-only module still produces a program`` () =
        let result = Helpers.parse "var x = 5;"
        Assert.Empty(result.Errors)
        Assert.True(result.Program.IsSome)
        Assert.Empty(result.Program.Value.Members)
        Assert.Single(result.Program.Value.Statements)

    [<Fact>]
    let ``Empty program stays empty`` () =
        let result = Helpers.parse ""
        Assert.Empty(result.Errors)
        Assert.True(result.Program.IsNone)

    [<Fact>]
    let ``Import standard binds the namespace name`` () =
        let result = Helpers.parse "import standard Terminal;"
        Assert.Empty(result.Errors)
        let import = result.Program.Value.Members.[0] :?> ImportStatementNode
        Assert.Equal(ImportKind.Standard, import.Kind)
        Assert.Equal(["Terminal"], import.Path)
        Assert.Null(import.Alias)

    [<Fact>]
    let ``Import module with alias`` () =
        let result = Helpers.parse "import module data as Data;"
        Assert.Empty(result.Errors)
        let import = result.Program.Value.Members.[0] :?> ImportStatementNode
        Assert.Equal(ImportKind.Module, import.Kind)
        Assert.Equal(["data"], import.Path)
        Assert.Equal("Data", import.Alias)

    [<Fact>]
    let ``Import external without alias`` () =
        let result = Helpers.parse "import external shared;"
        Assert.Empty(result.Errors)
        let import = result.Program.Value.Members.[0] :?> ImportStatementNode
        Assert.Equal(ImportKind.External, import.Kind)
        Assert.Null(import.Alias)

    [<Fact>]
    let ``Import with dotted module path`` () =
        let result = Helpers.parse "import module lib.core as Core;"
        Assert.Empty(result.Errors)
        let import = result.Program.Value.Members.[0] :?> ImportStatementNode
        Assert.Equal(["lib"; "core"], import.Path)

    [<Fact>]
    let ``Import without a source keyword is a syntax error`` () =
        let result = Helpers.parse "import Terminal;"
        Assert.True(
            result.Errors
            |> Seq.exists (fun e ->
                match e with
                | :? SyntaxError as s -> s.Kind = SyntaxErrorKind.ExpectedImportKind
                | _ -> false))

    [<Fact>]
    let ``Standard import rejects an alias`` () =
        let result = Helpers.parse "import standard Terminal as T;"
        Assert.True(
            result.Errors
            |> Seq.exists (fun e ->
                match e with
                | :? SyntaxError as s -> s.Kind = SyntaxErrorKind.UnexpectedAliasForStandardImport
                | _ -> false))

    [<Fact>]
    let ``Import missing semicolon is a syntax error`` () =
        let result = Helpers.parse "import standard Terminal"
        Assert.True(
            result.Errors
            |> Seq.exists (fun e ->
                match e with
                | :? SyntaxError as s -> s.Kind = SyntaxErrorKind.MissingSemicolonAfterImport
                | _ -> false))

    [<Fact>]
    let ``Private function is a member marked private`` () =
        let result = Helpers.parse "private func helper() : void { }"
        Assert.Empty(result.Errors)
        let functionDeclaration = result.Program.Value.Members.[0] :?> FunctionDeclarationNode
        Assert.True(functionDeclaration.IsPrivate)
        Assert.Equal("helper", functionDeclaration.Name)

    [<Fact>]
    let ``Private variable is a statement marked private`` () =
        let result = Helpers.parse "private var data = \"shared\";"
        Assert.Empty(result.Errors)
        let declaration = result.Program.Value.Statements.[0] :?> VariableDeclarationNode
        Assert.True(declaration.IsPrivate)
        Assert.False(declaration.IsConst)
        Assert.Equal("data", declaration.Name)

    [<Fact>]
    let ``Private typed and const declarations`` () =
        let result = Helpers.parse "private int count = 3;\nprivate const LIMIT = 10;"
        Assert.Empty(result.Errors)
        let typed = result.Program.Value.Statements.[0] :?> VariableDeclarationNode
        let constant = result.Program.Value.Statements.[1] :?> VariableDeclarationNode
        Assert.True(typed.IsPrivate)
        Assert.Equal(ScalarType.Int, typed.Type.ScalarType)
        Assert.True(constant.IsPrivate)
        Assert.True(constant.IsConst)
        Assert.Null(constant.Type)
        // The node anchors on 'private', not 'const' (line 2, column 1) —
        // same anchor as every other private declaration form.
        Assert.Equal(2, constant.StartLocation.Line)
        Assert.Equal(1, constant.StartLocation.Column)

    [<Fact>]
    let ``Declaration is public by default`` () =
        let result = Helpers.parse "func helper() : void { }"
        Assert.Empty(result.Errors)
        let functionDeclaration = result.Program.Value.Members.[0] :?> FunctionDeclarationNode
        Assert.False(functionDeclaration.IsPrivate)

    [<Fact>]
    let ``Private before a statement keyword is a syntax error`` () =
        let result = Helpers.parse "private if x { }"
        Assert.True(
            result.Errors
            |> Seq.exists (fun e ->
                match e with
                | :? SyntaxError as s -> s.Kind = SyntaxErrorKind.ExpectedDeclarationAfterPrivate
                | _ -> false))

    [<Fact>]
    let ``Inferred const declares without type or var`` () =
        let result = Helpers.parse "func main() : void { const greeting = \"ahoy\"; }"
        Assert.Empty(result.Errors)
        let functionDeclaration = result.Program.Value.Members.[0] :?> FunctionDeclarationNode
        let declaration = functionDeclaration.Body.Statements.[0] :?> VariableDeclarationNode
        Assert.True(declaration.IsConst)
        Assert.Null(declaration.Type)
        Assert.Equal("greeting", declaration.Name)
