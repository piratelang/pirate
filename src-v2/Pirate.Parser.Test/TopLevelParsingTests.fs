namespace Pirate.Parser.Test

open Pirate.Parser
open Pirate.Syntax.Nodes
open Xunit

module TopLevelParsingTests =

    [<Fact>]
    let ``Empty program`` () =
        let result = Helpers.parse ""
        Assert.Empty(result.Errors)
        Assert.True(result.Program.IsNone)

    [<Fact>]
    let ``Extern declaration`` () =
        let result = Helpers.parse "extern Standard.Terminal.Print;"
        Assert.Empty(result.Errors)
        let prog = result.Program.Value
        Assert.Single(prog.Members)
        let ext = prog.Members.[0] :?> ExternNode
        Assert.Equal(["Standard"; "Terminal"; "Print"], ext.QualifiedName)

    [<Fact>]
    let ``Function declaration`` () =
        let result = Helpers.parse "func main() : void { }"
        Assert.Empty(result.Errors)
        let prog = result.Program.Value
        Assert.Single(prog.Members)
        let fd = prog.Members.[0] :?> FunctionDeclarationNode
        Assert.Equal("main", fd.Name)
        Assert.Equal(ScalarType.Void, fd.ReturnType.ScalarType)
        Assert.Empty(fd.Body.Statements)

    [<Fact>]
    let ``Function with parameters`` () =
        let result = Helpers.parse "func add(int a, int b) : int { return a + b; }"
        Assert.Empty(result.Errors)
        let fd = (result.Program.Value.Members.[0]) :?> FunctionDeclarationNode
        Assert.Equal("add", fd.Name)
        Assert.Equal(2, fd.Parameters.Count)
        Assert.Equal("a", fd.Parameters.[0].Name)
        Assert.Equal(ScalarType.Int, fd.Parameters.[0].Type.ScalarType)
        Assert.Equal("b", fd.Parameters.[1].Name)

    [<Fact>]
    let ``Extern and function together`` () =
        let result = Helpers.parse "extern IO.print;\nfunc main() : void { print(1); }"
        Assert.Empty(result.Errors)
        let prog = result.Program.Value
        Assert.Equal(2, prog.Members.Count)
        Assert.IsType<ExternNode>(prog.Members.[0])
        Assert.IsType<FunctionDeclarationNode>(prog.Members.[1])

    [<Fact>]
    let ``Hello world program parses`` () =
        let source = """
extern Standard.Terminal.Print;

func main() : void
{
    Print("Hello World");
}
"""
        let result = Helpers.parse source
        Assert.Empty(result.Errors)
        Assert.True(result.Program.IsSome)
        let prog = result.Program.Value
        Assert.Equal(2, prog.Members.Count)
