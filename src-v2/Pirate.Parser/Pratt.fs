namespace Pirate.Parser

open System.Collections.Generic
open Pirate.Lexer
open Pirate.Syntax
open Pirate.Syntax.Nodes

/// <summary>
/// Pratt (precedence-climbing) expression parser per docs/GRAMMAR.md §3.5.
/// Handles all binary, unary, and postfix expression forms in a single
/// recursive framework driven by one precedence table.
/// </summary>
module internal Pratt =

    // --- Precedence table (binding powers, lowest to highest) ---

    let private precedence = function
        | TokenType.PipePipe       -> 1
        | TokenType.AmpAmp         -> 2
        | TokenType.EqualEqual
        | TokenType.BangEqual      -> 3
        | TokenType.Less
        | TokenType.LessEqual
        | TokenType.Greater
        | TokenType.GreaterEqual   -> 4
        | TokenType.Plus
        | TokenType.Minus          -> 5
        | TokenType.Star
        | TokenType.Slash
        | TokenType.Percent        -> 6
        | TokenType.Caret          -> 7  // right-associative
        | _                        -> 0

    let private isBinaryOperator = function
        | TokenType.PipePipe | TokenType.AmpAmp
        | TokenType.EqualEqual | TokenType.BangEqual
        | TokenType.Less | TokenType.LessEqual
        | TokenType.Greater | TokenType.GreaterEqual
        | TokenType.Plus | TokenType.Minus
        | TokenType.Star | TokenType.Slash | TokenType.Percent
        | TokenType.Caret -> true
        | _ -> false

    let private binaryOperatorOf = function
        | TokenType.Plus         -> BinaryOperator.Add
        | TokenType.Minus        -> BinaryOperator.Subtract
        | TokenType.Star         -> BinaryOperator.Multiply
        | TokenType.Slash        -> BinaryOperator.Divide
        | TokenType.Percent      -> BinaryOperator.Modulo
        | TokenType.Caret        -> BinaryOperator.Power
        | TokenType.EqualEqual   -> BinaryOperator.Equal
        | TokenType.BangEqual    -> BinaryOperator.NotEqual
        | TokenType.Less         -> BinaryOperator.Less
        | TokenType.LessEqual    -> BinaryOperator.LessEqual
        | TokenType.Greater      -> BinaryOperator.Greater
        | TokenType.GreaterEqual -> BinaryOperator.GreaterEqual
        | TokenType.AmpAmp       -> BinaryOperator.And
        | TokenType.PipePipe     -> BinaryOperator.Or
        | t                      -> failwithf "Not a binary operator: %A" t

    let private eof state = state.Pos >= state.Tokens.Count - 1
    let private peek state =
        if state.Pos >= state.Tokens.Count then state.Tokens.[state.Tokens.Count - 1]
        else state.Tokens.[state.Pos]
    // Advances carefully: never moves past the final Eof token, so a follow-up
    // peek is always in range even after an error path consumed the Eof token.
    let private advance state =
        let t = state.Tokens.[state.Pos]
        if state.Pos < state.Tokens.Count - 1 then state.Pos <- state.Pos + 1
        t
    let private at state t = not (eof state) && peek state |> fun tok -> tok.Type = t
    let private loc (t: Token) = t.Location

    // --- Prefix (unary + primary) ---
    let rec parsePrefix state : ExpressionNode =
        if eof state then
            state.Errors.Add(SyntaxError(SyntaxErrorKind.UnexpectedEofInExpression, "Unexpected end of input in expression", loc (peek state)))
            LiteralNode(loc (peek state), loc (peek state), LiteralKind.Int, 0) :> ExpressionNode
        else
            match (peek state).Type with
            | TokenType.Bang ->
                let op = advance state
                let operand = parsePrefix state
                UnaryOperationNode(loc op, operand.EndLocation, UnaryOperator.Not, operand) :> ExpressionNode
            | TokenType.Minus ->
                let op = advance state
                let operand = parsePrefix state
                UnaryOperationNode(loc op, operand.EndLocation, UnaryOperator.Negate, operand) :> ExpressionNode
            | _ -> parsePrimary state

    and parsePrimary state : ExpressionNode =
        if eof state then
            state.Errors.Add(SyntaxError(SyntaxErrorKind.UnexpectedEofInExpression, "Unexpected end of input in expression", loc (peek state)))
            LiteralNode(loc (peek state), loc (peek state), LiteralKind.Int, 0) :> ExpressionNode
        else
            match (peek state).Type with
            | TokenType.IntLiteral
            | TokenType.FloatLiteral
            | TokenType.StringLiteral
            | TokenType.CharLiteral ->
                let t = advance state
                let kind =
                    match t.Type with
                    | TokenType.IntLiteral    -> LiteralKind.Int
                    | TokenType.FloatLiteral  -> LiteralKind.Float
                    | TokenType.StringLiteral -> LiteralKind.String
                    | TokenType.CharLiteral   -> LiteralKind.Char
                    | _                       -> failwith "unreachable"
                LiteralNode(loc t, loc t, kind, t.Value) :> ExpressionNode

            | TokenType.True
            | TokenType.False ->
                let t = advance state
                LiteralNode(loc t, loc t, LiteralKind.Bool, t.Value) :> ExpressionNode

            | TokenType.LeftParen ->
                let openParen = advance state
                let expr = parseExpression state 0
                if at state TokenType.RightParen then
                    let closeParen = advance state
                    expr
                else
                    state.Errors.Add(SyntaxError(SyntaxErrorKind.MissingCloseParenAfterExpression, "Expected ')' after expression", loc openParen))
                    expr

            | TokenType.LeftBracket ->
                parseArrayLiteral state

            | TokenType.Identifier ->
                parseQualifiedName state

            | _ ->
                let tok = peek state
                state.Errors.Add(SyntaxError(SyntaxErrorKind.ExpectedExpression, sprintf "Expected expression, got '%s'" tok.Lexeme, loc tok))
                // Consume the offending token so the caller never spins on it.
                advance state |> ignore
                LiteralNode(loc tok, loc tok, LiteralKind.Int, 0) :> ExpressionNode

    and parseArrayLiteral state : ExpressionNode =
        let startTok = advance state // consume [
        let elements = ResizeArray<ExpressionNode>()
        if not (at state TokenType.RightBracket) then
            elements.Add(parseExpression state 0)
            while at state TokenType.Comma do
                advance state |> ignore
                if not (at state TokenType.RightBracket) then
                    elements.Add(parseExpression state 0)
        let endTok =
            if at state TokenType.RightBracket then advance state
            else
                state.Errors.Add(SyntaxError(SyntaxErrorKind.MissingCloseBracketAfterArrayLiteral, "Expected ']' to close array literal", loc startTok))
                startTok
        ArrayLiteralNode(loc startTok, loc endTok, elements) :> ExpressionNode

    and parseQualifiedName state : ExpressionNode =
        let startTok = peek state
        let parts = ResizeArray<string>()
        parts.Add(startTok.Lexeme)
        advance state |> ignore
        while at state TokenType.Dot do
            advance state |> ignore
            if at state TokenType.Identifier then
                parts.Add((advance state).Lexeme)
            else
                state.Errors.Add(SyntaxError(SyntaxErrorKind.MissingIdentifierAfterDot, "Expected identifier after '.'", loc (peek state)))
        QualifiedNameNode(loc startTok, loc (peek state), parts) :> ExpressionNode

    // --- Postfix (call, index) ---
    and applyPostfix state (expr: ExpressionNode) : ExpressionNode =
        if at state TokenType.LeftParen then
            let startLoc = expr.StartLocation
            advance state |> ignore // consume (
            let args = ResizeArray<ExpressionNode>()
            if not (at state TokenType.RightParen) then
                args.Add(parseExpression state 0)
                while at state TokenType.Comma do
                    advance state |> ignore
                    args.Add(parseExpression state 0)
            let closing =
                if at state TokenType.RightParen then advance state
                else
                    state.Errors.Add(SyntaxError(SyntaxErrorKind.MissingCloseParenAfterCall, "Expected ')' to close call arguments", loc (peek state)))
                    peek state
            let call = FunctionCallNode(startLoc, loc closing, expr, args)
            applyPostfix state (call :> ExpressionNode)
        elif at state TokenType.LeftBracket then
            let startLoc = expr.StartLocation
            advance state |> ignore // consume [
            let index = parseExpression state 0
            let closing =
                if at state TokenType.RightBracket then advance state
                else
                    state.Errors.Add(SyntaxError(SyntaxErrorKind.MissingCloseBracketAfterIndex, "Expected ']' to close index", loc (peek state)))
                    peek state
            let idx = IndexExpressionNode(startLoc, loc closing, expr, index)
            applyPostfix state (idx :> ExpressionNode)
        else
            expr

    // --- Main Pratt loop ---
    and parseExpression state minPrec : ExpressionNode =
        let mutable expr: ExpressionNode = parsePrefix state
        expr <- applyPostfix state expr

        while not (eof state) && isBinaryOperator (peek state).Type && precedence (peek state).Type >= minPrec do
            let opTok = peek state
            let prec = precedence opTok.Type
            let rightPrec = if opTok.Type = TokenType.Caret then prec else prec + 1
            advance state |> ignore
            let right = parseExpression state rightPrec
            expr <- BinaryOperationNode(expr.StartLocation, right.EndLocation, expr, binaryOperatorOf opTok.Type, right) :> ExpressionNode

        expr