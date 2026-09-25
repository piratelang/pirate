namespace Pirate.Parser.Test

open Pirate.Parser
open Xunit

module ErrorRecoveryTests =

    [<Fact>]
    let ``Missing semicolon produces diagnostic`` () =
        let result = Helpers.parse "func main() : void { var x = 5 }"
        Assert.NotEmpty(result.Errors)

    [<Fact>]
    let ``Mismatched braces produces diagnostic`` () =
        let result = Helpers.parse "func main() : void { var x = 5;"
        Assert.NotEmpty(result.Errors)

    [<Fact>]
    let ``Invalid top-level token produces diagnostic`` () =
        let result = Helpers.parse "bogus;"
        Assert.NotEmpty(result.Errors)

    [<Fact>]
    let ``Program still produced on error`` () =
        let result = Helpers.parse "func main() : void { var x = 5 }"
        // Even with errors, a partial program is produced
        Assert.True(result.Program.IsSome)

    [<Fact>]
    let ``Unterminated call arguments produces diagnostic`` () =
        let result = Helpers.parse "func main() : void { Print(42; }"
        Assert.NotEmpty(result.Errors)

    [<Fact>]
    let ``Missing closing paren in expression`` () =
        let result = Helpers.parse "func main() : void { var x = (1 + 2; }"
        Assert.NotEmpty(result.Errors)

    [<Fact>]
    let ``Missing function name`` () =
        let result = Helpers.parse "func () : void { }"
        Assert.NotEmpty(result.Errors)

    [<Fact>]
    let ``Missing return type colon`` () =
        let result = Helpers.parse "func main() void { }"
        Assert.NotEmpty(result.Errors)
