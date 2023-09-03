namespace Pirate.Lexer.F

open System
open Pirate.Common.Logger
open Pirate.Lexer.F.Tokens
open Pirate.Common.Logger.Enum
open Pirate.Lexer.F.Tokens.Enums

type ILexer =
    abstract member MakeTokens : string * string -> List<Token>
    abstract member CreateToken : List<Token> * Token -> List<Token>

type Lexer (logger:Logger, tokenRepository:TokenRepository) =
    let mutable _logger = logger
    let mutable _tokenRepository = tokenRepository

    let mutable _fileName = ""
    let mutable _position = 0

    interface ILexer with

        member this.CreateToken(tokens:List<Token>, token:Token) =
            let mutable tokens = tokens
            tokens <- tokens @ [token]
            _position <- _position + 1
            tokens

        member this.MakeTokens(text:string, fileName:string) : List<Token> =
            _logger.Log("Lexer started", LogType.INFO) |> ignore

            let mutable text = text.Replace("\n", "").Replace("\r", "").Replace("\t", "").Replace("    ", "")
            if text = null || text = "" then
                _logger.Log("Lexer text is null", LogType.INFO) |> ignore
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
                    tokens <- tokens @ [tokenResult.Token]
                    _position <- tokenResult.Position
                | _ when Char.IsLetter(text.[_position]) -> 
                    let tokenResult = _tokenRepository.MakeIdentifier(text, _position)
                    tokens <- tokens @ [tokenResult.Token]
                    _position <- tokenResult.Position
                | '"' -> 
                    let tokenResult = _tokenRepository.MakeString(text, _position)
                    tokens <- tokens @ [tokenResult.Token]
                    _position <- tokenResult.Position
                | '\'' -> 
                    let tokenResult = _tokenRepository.MakeChar(text, _position)
                    tokens <- tokens @ [tokenResult.Token]
                    _position <- tokenResult.Position
                | '+' -> 
                    let tokenResult = _tokenRepository.MakePlus(text, _position)
                    tokens <- tokens @ [tokenResult.Token]
                    _position <- tokenResult.Position
                | '-' -> tokens <- this.CreateToken(tokens, Token(OPERATORS, MINUS, "-"))
                | '*' -> tokens <- this.CreateToken(tokens, Token(OPERATORS, MULTIPLY, "*"))
                | '%' -> tokens <- this.CreateToken(tokens, Token(OPERATORS, MODULO, "%"))
                | '/' ->
                    let tokenResult = _tokenRepository.MakeDivide(text, _position)
                    tokens <- tokens @ [tokenResult.Token]
                    _position <- tokenResult.Position
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
                    tokens <- tokens @ [tokenResult.Token]
                    _position <- tokenResult.Position
                | '>' ->
                    let tokenResult = _tokenRepository.MakeGreaterThan(text, _position)
                    tokens <- tokens @ [tokenResult.Token]
                    _position <- tokenResult.Position
                | '<' ->
                    let tokenResult = _tokenRepository.MakeLessThan(text, _position)
                    tokens <- tokens @ [tokenResult.Token]
                    _position <- tokenResult.Position
                | '!' ->
                    let tokenResult = _tokenRepository.MakeNotEquals(text, _position)
                    tokens <- tokens @ [tokenResult.Token]
                    _position <- tokenResult.Position
                | _ -> 
                    tokens <- this.CreateToken(tokens, Token(TokenGroup.Empty, TokenType.Empty, text.[_position].ToString()))
                    _logger.Log(sprintf "Lexer: Unknown character: %s" (text.[_position].ToString()), LogType.INFO) |> ignore

                if _position >= text.Length then
                    Break <- true
        
            tokens
