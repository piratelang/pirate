namespace Pirate.Parser.Test

open Pirate.Parser
open Pirate.Syntax.Nodes
open Xunit

module StatementParsingTests =

    [<Fact>]
    let ``Variable declaration with var`` () =
        let result = Helpers.parse "func main() : void { var x = 5; }"
        Assert.Empty(result.Errors)
        let prog = result.Program.Value
        let fd = prog.Members.[0] :?> FunctionDeclarationNode
        let stmt = fd.Body.Statements.[0] :?> VariableDeclarationNode
        Assert.Equal("x", stmt.Name)
        Assert.Null(stmt.Type)

    [<Fact>]
    let ``Typed variable declaration`` () =
        let result = Helpers.parse "func main() : void { int x = 5; }"
        Assert.Empty(result.Errors)
        let fd = (result.Program.Value.Members.[0]) :?> FunctionDeclarationNode
        let stmt = fd.Body.Statements.[0] :?> VariableDeclarationNode
        Assert.Equal("x", stmt.Name)
        Assert.NotNull(stmt.Type)
        Assert.Equal(ScalarType.Int, stmt.Type.ScalarType)
        Assert.False(stmt.IsConst)

    [<Fact>]
    let ``Const typed declaration`` () =
        let result = Helpers.parse "func main() : void { const int limit = 10; }"
        Assert.Empty(result.Errors)
        let fd = (result.Program.Value.Members.[0]) :?> FunctionDeclarationNode
        let stmt = fd.Body.Statements.[0] :?> VariableDeclarationNode
        Assert.True(stmt.IsConst)
        Assert.Equal("limit", stmt.Name)
        Assert.Equal(ScalarType.Int, stmt.Type.ScalarType)

    [<Fact>]
    let ``Const var declaration`` () =
        let result = Helpers.parse "func main() : void { const var greeting = \"ahoy\"; }"
        Assert.Empty(result.Errors)
        let fd = (result.Program.Value.Members.[0]) :?> FunctionDeclarationNode
        let stmt = fd.Body.Statements.[0] :?> VariableDeclarationNode
        Assert.True(stmt.IsConst)
        Assert.Null(stmt.Type)

    [<Fact>]
    let ``Const starts node location`` () =
        let result = Helpers.parse "func main() : void { const int x = 5; }"
        let fd = (result.Program.Value.Members.[0]) :?> FunctionDeclarationNode
        let stmt = fd.Body.Statements.[0] :?> VariableDeclarationNode
        Assert.Equal(1, stmt.StartLocation.Line)
        Assert.Equal(22, stmt.StartLocation.Column)


    [<Fact>]
    let ``Array type declaration`` () =
        let result = Helpers.parse "func main() : void { int[] list = []; }"
        Assert.Empty(result.Errors)
        let fd = (result.Program.Value.Members.[0]) :?> FunctionDeclarationNode
        let stmt = fd.Body.Statements.[0] :?> VariableDeclarationNode
        Assert.True(stmt.Type.IsArray)

    [<Fact>]
    let ``Variable assignment`` () =
        let result = Helpers.parse "func main() : void { x = 5; }"
        Assert.Empty(result.Errors)
        let fd = (result.Program.Value.Members.[0]) :?> FunctionDeclarationNode
        let stmt = fd.Body.Statements.[0] :?> VariableAssignmentNode
        Assert.Equal("x", stmt.Name)

    [<Fact>]
    let ``If statement`` () =
        let result = Helpers.parse "func main() : void { if true { x = 1; } }"
        Assert.Empty(result.Errors)
        let fd = (result.Program.Value.Members.[0]) :?> FunctionDeclarationNode
        let stmt = fd.Body.Statements.[0] :?> IfStatementNode
        Assert.True(stmt.Condition :?> LiteralNode |> fun l -> l.Value :?> bool)
        Assert.NotNull(stmt.ThenBranch)

    [<Fact>]
    let ``If-else statement`` () =
        let result = Helpers.parse "func main() : void { if true { x = 1; } else { x = 2; } }"
        Assert.Empty(result.Errors)
        let fd = (result.Program.Value.Members.[0]) :?> FunctionDeclarationNode
        let ifStmt = fd.Body.Statements.[0] :?> IfStatementNode
        Assert.NotNull(ifStmt.ElseBranch)

    [<Fact>]
    let ``While loop`` () =
        let result = Helpers.parse "func main() : void { while true { x = 1; } }"
        Assert.Empty(result.Errors)
        let fd = (result.Program.Value.Members.[0]) :?> FunctionDeclarationNode
        let stmt = fd.Body.Statements.[0] :?> WhileStatementNode
        Assert.NotNull(stmt.Body)

    [<Fact>]
    let ``For counting loop`` () =
        let result = Helpers.parse "func main() : void { for var i = 0 to 10 { x = i; } }"
        Assert.Empty(result.Errors)
        let fd = (result.Program.Value.Members.[0]) :?> FunctionDeclarationNode
        let stmt = fd.Body.Statements.[0] :?> ForStatementNode
        Assert.Equal("i", stmt.VariableName)

    [<Fact>]
    let ``For-in loop`` () =
        let result = Helpers.parse "func main() : void { for (item in items) { x = item; } }"
        Assert.Empty(result.Errors)
        let fd = (result.Program.Value.Members.[0]) :?> FunctionDeclarationNode
        let stmt = fd.Body.Statements.[0] :?> ForInStatementNode
        Assert.Equal("item", stmt.VariableName)

    [<Fact>]
    let ``Return statement with value`` () =
        let result = Helpers.parse "func main() : int { return 42; }"
        Assert.Empty(result.Errors)
        let fd = (result.Program.Value.Members.[0]) :?> FunctionDeclarationNode
        let ret = fd.Body.ReturnStatement.Value
        Assert.Equal(42, (ret :?> LiteralNode).Value :?> int)

    [<Fact>]
    let ``Return statement without value`` () =
        let result = Helpers.parse "func main() : void { return; }"
        Assert.Empty(result.Errors)
        let fd = (result.Program.Value.Members.[0]) :?> FunctionDeclarationNode
        let ret = fd.Body.ReturnStatement
        Assert.Null(ret.Value)
