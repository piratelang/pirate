namespace Pirate.Parser

open System.Collections.Generic
open Pirate.Lexer
open Pirate.Syntax
open Pirate.Syntax.Nodes

/// <summary>
/// Top-level and statement parser that delegates to <see cref="Pratt"/> for
/// expressions. Recursive-descent for statements, Pratt for expressions.
/// Per docs/GRAMMAR.md §3.
/// </summary>
module Parser =

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

    let private expect state tokenType kind msg =
        if at state tokenType then
            Some (advance state)
        else
            state.Errors.Add(SyntaxError(kind, msg, loc (peek state)))
            None

    // --- Type ---
    let private parseType state : TypeNode option =
        let scalarType =
            match (peek state).Type with
            | TokenType.Int    -> Some ScalarType.Int
            | TokenType.Float  -> Some ScalarType.Float
            | TokenType.String -> Some ScalarType.String
            | TokenType.Char   -> Some ScalarType.Char
            | TokenType.Bool   -> Some ScalarType.Bool
            | TokenType.Void   -> Some ScalarType.Void
            | _ ->
                let tok = peek state
                state.Errors.Add(SyntaxError(SyntaxErrorKind.ExpectedType, sprintf "Expected type, got '%s'" tok.Lexeme, loc tok))
                None
        match scalarType with
        | None -> None
        | Some st ->
            let startTok = peek state
            advance state |> ignore // consume the scalar keyword
            let mutable isArr = false
            if at state TokenType.LeftBracket then
                advance state |> ignore
                if at state TokenType.RightBracket then
                    advance state |> ignore
                    isArr <- true
                else
                    state.Errors.Add(SyntaxError(SyntaxErrorKind.MissingCloseBracketInType, "Expected ']' after '[' in type", loc (peek state)))
            // End at the last consumed token (the scalar keyword or ']'), not the next token.
            let endLoc = loc (state.Tokens.[state.Pos - 1])
            Some (TypeNode(loc startTok, endLoc, st, isArr))

    // --- Block ---
    let rec private parseBlock state : BlockNode option =
        match expect state TokenType.LeftBrace SyntaxErrorKind.MissingOpenBrace "Expected '{' to begin block" with
        | None -> None
        | Some openBrace ->
            let stmts = ResizeArray<StatementNode>()
            let mutable retNode: ReturnStatementNode = Unchecked.defaultof<_>
            while not (eof state) && not (at state TokenType.RightBrace) do
                match parseStatement state with
                | Some (:? ReturnStatementNode as rs) -> retNode <- rs
                | Some s -> stmts.Add(s)
                | None -> advance state |> ignore
            let closeBrace =
                match expect state TokenType.RightBrace SyntaxErrorKind.MissingCloseBrace "Expected '}' to close block" with
                | Some t -> t
                | None -> openBrace
            // C# nullable BlockNode.ReturnStatement: retNode stays null when no return was seen.
            Some (BlockNode(loc openBrace, loc closeBrace, stmts, retNode))

    // --- Statements ---
    and private parseStatement state : StatementNode option =
        if eof state || at state TokenType.RightBrace then
            None
        else
            match (peek state).Type with
            | TokenType.Return -> parseReturn state
            | TokenType.If     -> parseIf state
            | TokenType.While  -> parseWhile state
            | TokenType.For    -> parseFor state
            | TokenType.Var    -> parseVarDecl state
            | TokenType.Int
            | TokenType.Float
            | TokenType.String
            | TokenType.Char
            | TokenType.Bool   -> parseTypedVarDecl state
            | _                -> parseExprStmt state

    and private parseReturn state : StatementNode option =
        let startTok = advance state
        // C# nullable ReturnStatementNode.Value: stays null when the return has no value.
        let mutable valueNode: ExpressionNode = Unchecked.defaultof<_>
        if not (at state TokenType.Semicolon) then
            valueNode <- Pratt.parseExpression state 0
        expect state TokenType.Semicolon SyntaxErrorKind.MissingSemicolonAfterReturn "Expected ';' after return" |> ignore
        Some (ReturnStatementNode(loc startTok, loc (peek state), valueNode) :> StatementNode)

    and private parseIf state : StatementNode option =
        let startTok = advance state
        let cond = Pratt.parseExpression state 0
        match parseBlock state with
        | None -> None
        | Some thenBlock ->
            // C# nullable IfStatementNode.ElseBranch: null when there is no else.
            let mutable elseBranch: StatementNode = Unchecked.defaultof<_>
            if at state TokenType.Else then
                advance state |> ignore
                if at state TokenType.If then
                    match parseIf state with
                    | Some nested -> elseBranch <- nested
                    | None -> ()
                else
                    match parseBlock state with
                    | Some b -> elseBranch <- b :> StatementNode
                    | None -> ()
            Some (IfStatementNode(loc startTok, loc (peek state), cond, thenBlock, elseBranch) :> StatementNode)

    and private parseWhile state : StatementNode option =
        let startTok = advance state
        let cond = Pratt.parseExpression state 0
        match parseBlock state with
        | Some body -> Some (WhileStatementNode(loc startTok, loc (peek state), cond, body) :> StatementNode)
        | None -> None

    and private parseFor state : StatementNode option =
        let startTok = advance state
        if at state TokenType.LeftParen then
            parseForIn state startTok
        elif at state TokenType.Var then
            parseForCounting state startTok
        else
            state.Errors.Add(SyntaxError(SyntaxErrorKind.MissingVarOrParenAfterFor, "Expected 'var' or '(' after 'for'", loc (peek state)))
            None

    and private parseForIn state startTok : StatementNode option =
        advance state |> ignore // consume (
        match expect state TokenType.Identifier SyntaxErrorKind.MissingIdentifierInForIn "Expected identifier in for-in" with
        | None -> None
        | Some id ->
            if not (at state TokenType.In) then
                state.Errors.Add(SyntaxError(SyntaxErrorKind.MissingInInForIn, "Expected 'in' in for-in", loc (peek state)))
                None
            else
                advance state |> ignore
                let iter = Pratt.parseExpression state 0
                expect state TokenType.RightParen SyntaxErrorKind.MissingCloseParenInForIn "Expected ')' in for-in" |> ignore
                match parseBlock state with
                | Some body -> Some (ForInStatementNode(loc startTok, loc (peek state), id.Lexeme, iter, body) :> StatementNode)
                | None -> None

    and private parseForCounting state startTok : StatementNode option =
        advance state |> ignore // consume var
        match expect state TokenType.Identifier SyntaxErrorKind.MissingIdentifierInFor "Expected identifier in for" with
        | None -> None
        | Some id ->
            if not (at state TokenType.Equal) then
                state.Errors.Add(SyntaxError(SyntaxErrorKind.MissingEqualsInFor, "Expected '=' in for", loc (peek state)))
                None
            else
                advance state |> ignore
                let startExpr = Pratt.parseExpression state 0
                if not (at state TokenType.To) then
                    state.Errors.Add(SyntaxError(SyntaxErrorKind.MissingToInFor, "Expected 'to' in for", loc (peek state)))
                    None
                else
                    advance state |> ignore
                    let endExpr = Pratt.parseExpression state 0
                    match parseBlock state with
                    | Some body -> Some (ForStatementNode(loc startTok, loc (peek state), id.Lexeme, startExpr, endExpr, body) :> StatementNode)
                    | None -> None

    and private parseVarDecl state : StatementNode option =
        let startTok = advance state // consume var
        match expect state TokenType.Identifier SyntaxErrorKind.MissingIdentifierAfterVar "Expected identifier after 'var'" with
        | None -> None
        | Some id ->
            if not (at state TokenType.Equal) then
                state.Errors.Add(SyntaxError(SyntaxErrorKind.MissingEqualsInDeclaration, "Expected '=' in variable declaration", loc (peek state)))
                None
            else
                advance state |> ignore
                let init = Pratt.parseExpression state 0
                expect state TokenType.Semicolon SyntaxErrorKind.MissingSemicolonAfterDeclaration "Expected ';' after declaration" |> ignore
                // 'var' declarations have no explicit type: pass null for C# nullable VariableDeclarationNode.Type.
                Some (VariableDeclarationNode(loc startTok, loc (peek state), null, id.Lexeme, init) :> StatementNode)

    and private parseTypedVarDecl state : StatementNode option =
        let startTok = peek state
        match parseType state with
        | None -> None
        | Some typ ->
            match expect state TokenType.Identifier SyntaxErrorKind.MissingIdentifierAfterType "Expected identifier after type" with
            | None -> None
            | Some id ->
                if not (at state TokenType.Equal) then
                    state.Errors.Add(SyntaxError(SyntaxErrorKind.MissingEqualsInDeclaration, "Expected '=' in variable declaration", loc (peek state)))
                    None
                else
                    advance state |> ignore
                    let init = Pratt.parseExpression state 0
                    expect state TokenType.Semicolon SyntaxErrorKind.MissingSemicolonAfterDeclaration "Expected ';' after declaration" |> ignore
                    Some (VariableDeclarationNode(loc startTok, loc (peek state), typ, id.Lexeme, init) :> StatementNode)

    and private parseExprStmt state : StatementNode option =
        let startTok = peek state
        let expr = Pratt.parseExpression state 0
        if at state TokenType.Equal then
            // assignment-statement: identifier [ '[' expression ']' ] '=' expression ';'
            advance state |> ignore
            let value = Pratt.parseExpression state 0
            expect state TokenType.Semicolon SyntaxErrorKind.MissingSemicolonAfterExpression "Expected ';' after expression" |> ignore
            match expr with
            | :? QualifiedNameNode as qn when qn.Parts.Count = 1 ->
                Some (VariableAssignmentNode(qn.StartLocation, loc (peek state), qn.Parts.[0], null, value) :> StatementNode)
            | :? IndexExpressionNode as idx ->
                match idx.Target with
                | :? QualifiedNameNode as target when target.Parts.Count = 1 ->
                    Some (VariableAssignmentNode(idx.StartLocation, loc (peek state), target.Parts.[0], idx.Index, value) :> StatementNode)
                | _ ->
                    state.Errors.Add(SyntaxError(SyntaxErrorKind.ExpectedExpression, "Invalid assignment target", idx.StartLocation))
                    Some (ExpressionStatementNode(loc startTok, loc (peek state), expr) :> StatementNode)
            | _ ->
                state.Errors.Add(SyntaxError(SyntaxErrorKind.ExpectedExpression, "Invalid assignment target", expr.StartLocation))
                Some (ExpressionStatementNode(loc startTok, loc (peek state), expr) :> StatementNode)
        else
            expect state TokenType.Semicolon SyntaxErrorKind.MissingSemicolonAfterExpression "Expected ';' after expression" |> ignore
            Some (ExpressionStatementNode(loc startTok, loc (peek state), expr) :> StatementNode)

    // --- Top-level ---
    let private parseExtern state : TopLevelNode =
        let startTok = advance state
        let parts = ResizeArray<string>()
        match expect state TokenType.Identifier SyntaxErrorKind.MissingIdentifierAfterExtern "Expected identifier after 'extern'" with
        | None -> ()
        | Some id ->
            parts.Add(id.Lexeme)
            while at state TokenType.Dot do
                advance state |> ignore
                match expect state TokenType.Identifier SyntaxErrorKind.MissingIdentifierAfterDot "Expected identifier after '.'" with
                | None -> ()
                | Some id2 -> parts.Add(id2.Lexeme)
        expect state TokenType.Semicolon SyntaxErrorKind.MissingSemicolonAfterDeclaration "Expected ';' after extern" |> ignore
        ExternNode(loc startTok, loc (peek state), parts)

    let rec private parseTopLevel state : TopLevelNode list =
        if eof state then
            []
        else
            match (peek state).Type with
            | TokenType.Extern ->
                parseExtern state :: parseTopLevel state
            | TokenType.Func ->
                parseFuncDecl state :: parseTopLevel state
            | _ ->
                let tok = peek state
                state.Errors.Add(SyntaxError(SyntaxErrorKind.ExpectedTopLevelDeclaration, sprintf "Expected 'extern' or 'func', got '%s'" tok.Lexeme, loc tok))
                advance state |> ignore
                parseTopLevel state

    and private parseFuncDecl state : TopLevelNode =
        let startTok = advance state
        let name =
            match expect state TokenType.Identifier SyntaxErrorKind.MissingFunctionName "Expected function name" with
            | Some id -> id.Lexeme
            | None -> "???"
        expect state TokenType.LeftParen SyntaxErrorKind.MissingOpenParenAfterFunctionName "Expected '(' after function name" |> ignore
        let parameters = ResizeArray<ParameterDefinitionNode>()
        if not (at state TokenType.RightParen) then
            parseParam state |> Option.iter parameters.Add
            while at state TokenType.Comma do
                advance state |> ignore
                parseParam state |> Option.iter parameters.Add
        expect state TokenType.RightParen SyntaxErrorKind.MissingCloseParenAfterParameters "Expected ')' after parameters" |> ignore
        expect state TokenType.Colon SyntaxErrorKind.MissingColonBeforeReturnType "Expected ':' before return type" |> ignore
        let retType =
            match parseType state with
            | Some t -> t
            | None -> TypeNode(loc (peek state), loc (peek state), ScalarType.Void, false)
        let body =
            match parseBlock state with
            | Some b -> b
            | None -> BlockNode(loc (peek state), loc (peek state), ResizeArray<StatementNode>(), null)
        FunctionDeclarationNode(loc startTok, loc (peek state), name, parameters, retType, body)

    and private parseParam state : ParameterDefinitionNode option =
        match parseType state with
        | None -> None
        | Some typ ->
            match expect state TokenType.Identifier SyntaxErrorKind.MissingParameterName "Expected parameter name" with
            | None -> None
            | Some id -> Some (ParameterDefinitionNode(typ.StartLocation, loc id, typ, id.Lexeme))

    // --- Entry point ---

    /// Parses a token stream into a <see cref="ParseResult"/>.
    let Parse (lexResult: LexResult) : ParseResult =
        let state =
            { Tokens = lexResult.Tokens
              Pos = 0
              // Syntax errors only — lexer errors stay in LexResult.Errors;
              // the CLI concatenates both lists for display.
              Errors = ResizeArray<CompilationError>() }
        let topLevel = parseTopLevel state
        let program =
            match topLevel with
            | [] -> None
            | nodes ->
                let firstLoc = nodes.Head.StartLocation
                let lastLoc = (List.last nodes).EndLocation
                Some (ProgramNode(firstLoc, lastLoc, ResizeArray<TopLevelNode>(nodes)))
        ParseResult(program, state.Errors)