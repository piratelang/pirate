namespace Pirate.Parser.Test

open Pirate.Lexer
open Pirate.Parser
open Pirate.Syntax
open Pirate.Syntax.Nodes
open Xunit

module Helpers =
    let parse source =
        let lexResult = Lexer().Tokenize source
        Parser.Parse lexResult PirateFileKind.Module

    let getExpr source =
        // Expressions only exist inside a function body; wrap the snippet so it
        // parses as a top-level program holding a single expression statement.
        let result = parse ("func main() : void { " + source + " }")
        Assert.Empty(result.Errors)
        Assert.True(result.Program.IsSome)
        let prog = result.Program.Value
        // Find the first expression statement
        let rec findFirstExpr (nodes: seq<TopLevelNode>) =
            seq {
                for node in nodes do
                    match node with
                    | :? FunctionDeclarationNode as fd ->
                        yield! findFirstStmt fd.Body.Statements
                    | _ -> ()
            }
        and findFirstStmt (stmts: seq<StatementNode>) =
            seq {
                for stmt in stmts do
                    match stmt with
                    | :? ExpressionStatementNode as es -> yield es.Expression
                    | :? BlockNode as block -> yield! findFirstStmt block.Statements
                    | _ -> ()
            }
        (findFirstExpr prog.Members |> Seq.head)

module ExpressionParsingTests =

    [<Fact>]
    let ``Literal integers`` () =
        let expr = Helpers.getExpr "42;"
        let lit = expr :?> LiteralNode
        Assert.Equal(LiteralKind.Int, lit.LiteralKind)
        Assert.Equal(42, lit.Value :?> int)

    [<Fact>]
    let ``Literal floats`` () =
        let expr = Helpers.getExpr "3.14;"
        let lit = expr :?> LiteralNode
        Assert.Equal(LiteralKind.Float, lit.LiteralKind)
        Assert.Equal(3.14, lit.Value :?> float)

    [<Fact>]
    let ``Literal strings`` () =
        let expr = Helpers.getExpr "\"ahoy\";"
        let lit = expr :?> LiteralNode
        Assert.Equal(LiteralKind.String, lit.LiteralKind)
        Assert.Equal("ahoy", lit.Value :?> string)

    [<Fact>]
    let ``Literal bools`` () =
        let t = Helpers.getExpr "true;" :?> LiteralNode
        Assert.Equal(LiteralKind.Bool, t.LiteralKind)
        Assert.Equal(true, t.Value :?> bool)
        let f = Helpers.getExpr "false;" :?> LiteralNode
        Assert.Equal(false, f.Value :?> bool)

    [<Fact>]
    let ``Simple addition`` () =
        let expr = Helpers.getExpr "1 + 2;"
        let bin = expr :?> BinaryOperationNode
        Assert.Equal(BinaryOperator.Add, bin.Operator)
        Assert.Equal(1, (bin.Left :?> LiteralNode).Value :?> int)
        Assert.Equal(2, (bin.Right :?> LiteralNode).Value :?> int)

    [<Fact>]
    let ``Operator precedence â€” multiplication before addition`` () =
        let expr = Helpers.getExpr "1 + 2 * 3;"
        let bin = expr :?> BinaryOperationNode
        // + is root, left=1, right=2*3
        Assert.Equal(BinaryOperator.Add, bin.Operator)
        Assert.Equal(1, (bin.Left :?> LiteralNode).Value :?> int)
        let rightBin = bin.Right :?> BinaryOperationNode
        Assert.Equal(BinaryOperator.Multiply, rightBin.Operator)

    [<Fact>]
    let ``Operator precedence â€” power is right-associative`` () =
        // 2 ^ 3 ^ 4 should be 2^(3^4), not (2^3)^4
        let expr = Helpers.getExpr "2 ^ 3 ^ 4;"
        let bin = expr :?> BinaryOperationNode
        Assert.Equal(BinaryOperator.Power, bin.Operator)
        // Left is 2, right is 3^4
        Assert.Equal(2, (bin.Left :?> LiteralNode).Value :?> int)
        let rightBin = bin.Right :?> BinaryOperationNode
        Assert.Equal(BinaryOperator.Power, rightBin.Operator)

    [<Fact>]
    let ``Unary negation`` () =
        let expr = Helpers.getExpr "-5;"
        let unary = expr :?> UnaryOperationNode
        Assert.Equal(UnaryOperator.Negate, unary.Operator)
        Assert.Equal(5, (unary.Operand :?> LiteralNode).Value :?> int)

    [<Fact>]
    let ``Logical not`` () =
        let expr = Helpers.getExpr "!true;"
        let unary = expr :?> UnaryOperationNode
        Assert.Equal(UnaryOperator.Not, unary.Operator)

    [<Fact>]
    let ``Parenthesized expression`` () =
        let expr = Helpers.getExpr "(1 + 2) * 3;"
        let bin = expr :?> BinaryOperationNode
        Assert.Equal(BinaryOperator.Multiply, bin.Operator)
        let leftBin = bin.Left :?> BinaryOperationNode
        Assert.Equal(BinaryOperator.Add, leftBin.Operator)

    [<Fact>]
    let ``Function call`` () =
        let expr = Helpers.getExpr "Print(42);"
        let call = expr :?> FunctionCallNode
        Assert.IsType<QualifiedNameNode>(call.Callee)
        Assert.Single(call.Arguments)
        Assert.Equal(42, (call.Arguments.[0] :?> LiteralNode).Value :?> int)

    [<Fact>]
    let ``Array literal`` () =
        let expr = Helpers.getExpr "[1, 2, 3];"
        let arr = expr :?> ArrayLiteralNode
        Assert.Equal(3, arr.Elements.Count)

    [<Fact>]
    let ``Array indexing`` () =
        let expr = Helpers.getExpr "arr[0];"
        let idx = expr :?> IndexExpressionNode
        Assert.IsType<QualifiedNameNode>(idx.Target)
        Assert.Equal(0, (idx.Index :?> LiteralNode).Value :?> int)

    [<Fact>]
    let ``Chained call and index: f()[0]`` () =
        let expr = Helpers.getExpr "getArray()[0];"
        let idx = expr :?> IndexExpressionNode
        Assert.IsType<FunctionCallNode>(idx.Target)

    [<Fact>]
    let ``Comparison operators`` () =
        let expr = Helpers.getExpr "1 < 2;"
        let bin = expr :?> BinaryOperationNode
        Assert.Equal(BinaryOperator.Less, bin.Operator)

    [<Fact>]
    let ``Logical and has higher precedence than or`` () =
        let expr = Helpers.getExpr "a || b && c;"
        let bin = expr :?> BinaryOperationNode
        Assert.Equal(BinaryOperator.Or, bin.Operator)
        Assert.Equal(BinaryOperator.And, (bin.Right :?> BinaryOperationNode).Operator)

    // --- Member access postfix (docs/GRAMMAR.md §3.6/§4.3) ---

    [<Fact>]
    let ``Member access`` () =
        let expr = Helpers.getExpr "c.value;"
        let access = expr :?> MemberAccessNode
        Assert.Equal("value", access.Member)
        let target = access.Target :?> QualifiedNameNode
        Assert.Equal("c", target.Name)

    [<Fact>]
    let ``Dotted call builds nested member access, not one qualified name`` () =
        let expr = Helpers.getExpr "Standard.Terminal.Print(x);"
        let call = expr :?> FunctionCallNode
        let print = call.Callee :?> MemberAccessNode
        Assert.Equal("Print", print.Member)
        let terminal = print.Target :?> MemberAccessNode
        Assert.Equal("Terminal", terminal.Member)
        let standard = terminal.Target :?> QualifiedNameNode
        Assert.Equal("Standard", standard.Name)

    [<Fact>]
    let ``Member access on a call target`` () =
        let expr = Helpers.getExpr "getItem().name;"
        let access = expr :?> MemberAccessNode
        Assert.Equal("name", access.Member)
        Assert.IsType<FunctionCallNode>(access.Target)
