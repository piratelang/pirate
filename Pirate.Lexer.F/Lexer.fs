namespace Pirate.Lexer

open System
open Pirate.Common.Logger
open Pirate.Lexer.Tokens
open Pirate.Common.Logger.Enum
open Pirate.Lexer.Enums
open Pirate.Common.Logger.Interfaces

type Lexer (logger:ILogger, tokenRepository:TokenRepository) =
    let mutable _logger = logger
    let mutable _tokenRepository = tokenRepository

    let mutable _fileName = ""
    let mutable _position = 0

    member private this.CreateToken(tokens:List<Token>, token:Token) =
        
        let mutable tokens = tokens
        tokens <- tokens @ [token]
        _position <- _position + 1

        _logger.Debug($"Created token: {token.ToString()}") |> ignore
        tokens

    member private this.ParseTokenResult(tokens:List<Token>, tokenResult:TokenResult) =
        let mutable tokens = tokens
        tokens <- tokens @ [tokenResult.Token]
        _position <- tokenResult.Position

        _logger.Debug($"Created token: {tokenResult.Token.ToString()}") |> ignore
        tokens

    member this.MakeTokens(text:string, fileName:string) : List<Token> =
        _logger.Info("Lexer started") |> ignore

        let mutable text = text.Replace("\n", "").Replace("\r", "").Replace("\t", "").Replace("    ", "")
        if text = null || text = "" then
            _logger.Info("Lexer text is null") |> ignore
            raise (System.Exception("Lexer text is null"))

        _position <- 0
        _fileName <- fileName

        let mutable tokens : List<Token> = []
        let mutable Break = false

        while not Break do
            if _position >= text.Length then
                Break <- true

            match text.[_position] with
            | ' ' -> _position <- _position + 1
            | _ when Char.IsDigit(text.[_position]) -> 
                let tokenResult = _tokenRepository.MakeNumber(text, _position)
                tokens <- this.ParseTokenResult(tokens, tokenResult)
            | _ when Char.IsLetter(text.[_position]) -> 
                let tokenResult = _tokenRepository.MakeIdentifier(text, _position)
                tokens <- this.ParseTokenResult(tokens, tokenResult)
            | '"' -> 
                let tokenResult = _tokenRepository.MakeString(text, _position)
                tokens <- this.ParseTokenResult(tokens, tokenResult)
            | '\'' -> 
                let tokenResult = _tokenRepository.MakeChar(text, _position)
                tokens <- this.ParseTokenResult(tokens, tokenResult)
            | '+' -> 
                let tokenResult = _tokenRepository.MakePlus(text, _position)
                tokens <- this.ParseTokenResult(tokens, tokenResult)
            | '-' -> tokens <- this.CreateToken(tokens, Token(OPERATORS, MINUS, "-"))
            | '*' -> tokens <- this.CreateToken(tokens, Token(OPERATORS, MULTIPLY, "*"))
            | '%' -> tokens <- this.CreateToken(tokens, Token(OPERATORS, MODULO, "%"))
            | '/' ->
                let tokenResult = _tokenRepository.MakeDivide(text, _position)
                tokens <- this.ParseTokenResult(tokens, tokenResult)
            | '^' -> tokens <- this.CreateToken(tokens, Token(OPERATORS, POWER, "^"))
            | '(' -> tokens <- this.CreateToken(tokens, Token(SYNTAX, LEFTPARENTHESES, "("))
            | ')' -> tokens <- this.CreateToken(tokens, Token(SYNTAX, RIGHTPARENTHESES, ")"))
            | '{' -> tokens <- this.CreateToken(tokens, Token(SYNTAX, LEFTCURLYBRACE, "{"))
            | '}' -> tokens <- this.CreateToken(tokens, Token(SYNTAX, RIGHTCURLYBRACE, "}"))
            | ',' -> tokens <- this.CreateToken(tokens, Token(SYNTAX, COMMA, ","))
            | ':' -> tokens <- this.CreateToken(tokens, Token(SYNTAX, COLON, ":"))
            | ';' -> tokens <- this.CreateToken(tokens, Token(SYNTAX, SEMICOLON, ";"))
            | '.' -> tokens <- this.CreateToken(tokens, Token(SYNTAX, DOT, "."))
            | '$' -> tokens <- this.CreateToken(tokens, Token(SYNTAX, DOLLAR, "$"))
            | '[' -> tokens <- this.CreateToken(tokens, Token(SYNTAX, LEFTBRACKET, "["))
            | ']' -> tokens <- this.CreateToken(tokens, Token(SYNTAX, RIGHTBRACKET, "]"))
            | '=' ->
                let tokenResult = _tokenRepository.MakeEquals(text, _position)
                tokens <- this.ParseTokenResult(tokens, tokenResult)
            | '>' ->
                let tokenResult = _tokenRepository.MakeGreaterThan(text, _position)
                tokens <- this.ParseTokenResult(tokens, tokenResult)
            | '<' ->
                let tokenResult = _tokenRepository.MakeLessThan(text, _position)
                tokens <- this.ParseTokenResult(tokens, tokenResult)
            | '!' ->
                let tokenResult = _tokenRepository.MakeNotEquals(text, _position)
                tokens <- this.ParseTokenResult(tokens, tokenResult)
            | _ -> 
                tokens <- this.CreateToken(tokens, Token(TokenGroup.Empty, TokenType.Empty, text.[_position].ToString()))
                _logger.Warning(sprintf "Lexer: Unknown character: %s" (text.[_position].ToString())) |> ignore

            if _position >= text.Length then
                Break <- true
        
        tokens
