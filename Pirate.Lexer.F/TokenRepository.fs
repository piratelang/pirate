namespace Pirate.Lexer.F

open Pirate.Lexer.F.Tokens
open Pirate.Lexer.F.Tokens.Enums
open System
open System.Globalization
open System.Collections.Generic

type ITokenRepository =
    abstract member MakeNumber : string * int -> TokenResult
    abstract member MakeIdentifier : string * int -> TokenResult
    abstract member MakeString : string * int -> TokenResult
    abstract member MakeChar : string * int -> TokenResult
    abstract member MakeNotEquals : string * int -> TokenResult
    abstract member MakeGreaterThan : string * int -> TokenResult
    abstract member MakeLessThan : string * int -> TokenResult
    abstract member MakeEquals : string * int -> TokenResult
    abstract member MakePlus : string * int -> TokenResult
    abstract member MakeDivide : string * int -> TokenResult

type TokenRepository(keyWordService:IKeyWordService) =
    let mutable _keyWordService = keyWordService

    interface ITokenRepository with
        member _.MakeNumber(text:string, position:int) : TokenResult =
            let mutable position = position

            let mutable dotCount = 0
            let mutable numberString = "";
            let mutable token = Token(TokenGroup.VALUE, TokenType.Empty, 0)

            let mutable Break = false
            while not Break && (Char.IsDigit(text.[position]) || text.[position] = '.') do
                if text.[position] = '.' then
                    if dotCount = 1 then
                        Break <- true
                    else
                        dotCount <- dotCount + 1
                        numberString <- numberString + "."
                else
                    numberString <- numberString + text.[position].ToString()

                position <- position + 1
                if position = text.Length then
                    Break <- true

            if dotCount = 0 then
                token <- Token(TokenGroup.VALUE, TokenType.INT, int numberString)
            else
                let cultureInfo = CultureInfo.CurrentCulture.Clone() :?> CultureInfo
                cultureInfo.NumberFormat.CurrencyDecimalSeparator <- "."
                cultureInfo.NumberFormat.NumberDecimalSeparator <- "."
            
                match Double.TryParse(numberString, NumberStyles.Number, CultureInfo.InvariantCulture) with
                | (true, result) -> token <- Token(TokenGroup.VALUE, TokenType.FLOAT, result)
                | (false, result) -> raise (System.Exception("Invalid number format"))

            TokenResult(token, position)

        member _.MakeIdentifier(text:string, position:int) : TokenResult =
            let mutable position = position

            let mutable idString = "";

            let mutable Break = false
            while not Break && (Char.IsLetter(text.[position]) || Char.IsDigit(text.[position]) || Char.IsWhiteSpace(text.[position]) || Char.IsSeparator(text.[position])) do
                idString <- idString + text.[position].ToString()
                position <- position + 1
                if position = text.Length then
                    Break <- true
                else if Char.IsNumber(text.[position]) || Char.IsDigit(text.[position]) || Char.IsWhiteSpace(text.[position]) || Char.IsSeparator(text.[position]) then
                    Break <- true

            let mutable TokenTypeType = _keyWordService.GetTypeKeyword(idString)
            let mutable TokenTypeControl = _keyWordService.GetTokenControlKeyword(idString)
            if TokenTypeType <> TokenType.Empty then
                TokenResult(
                    Token(TokenGroup.TYPEKEYWORD, TokenTypeType, idString), 
                    position
                )
            else if TokenTypeControl <> TokenType.Empty then
                TokenResult(
                    Token(TokenGroup.CONTROLKEYWORD, TokenTypeControl, idString), 
                    position
                )
            else 
                TokenResult(
                    Token(TokenGroup.SYNTAX, TokenType.IDENTIFIER, idString), 
                    position
                )

        member _.MakeString(text:string, position:int) : TokenResult =
            let mutable position = position + 1

            let mutable resultString = "";
            let mutable escapeCharacter = false

            let escapeCharacters = new Dictionary<string, string>();
            escapeCharacters.Add("n", "\n");
            escapeCharacters.Add("t", "\t");

            let mutable Break = false
            while not Break && text.[position] <> '"' do
                if escapeCharacter then
                    resultString <- resultString + escapeCharacters.[text.[position].ToString()]
                else
                    if text.[position] = '\\' then
                        escapeCharacter <- true
                    else
                        resultString <- resultString + text.[position].ToString()
                position <- position + 1
                escapeCharacter <- false
                if position = text.Length then
                    Break <- true

            position <- position + 1

            TokenResult(
                Token(TokenGroup.VALUE, TokenType.STRING, resultString), 
                position
            )

        member _.MakeChar(text:string, position:int) : TokenResult =
            let mutable position = position + 1

            let mutable resultString = text.[position].ToString()
            position <- position + 1
            if text.[position] <> '\'' then
                raise (System.Exception("Invalid char format"))
            position <- position + 1

            TokenResult(
                Token(TokenGroup.VALUE, TokenType.CHAR, resultString), 
                position
            )

        member _.MakeNotEquals(text:string, position:int) : TokenResult =
            let mutable position = position + 1

            if text.[position] <> '=' then
                raise (System.Exception("Invalid not equals format"))

            position <- position + 1
            TokenResult(
                Token(TokenGroup.COMPARISONOPERATORS, TokenType.NOTEQUALS, "!="),
                position
            )

        member _.MakeGreaterThan(text:string, position:int) : TokenResult =
            let mutable position = position + 1

            try
                if text.[position] = '=' then
                    position <- position + 1
                    TokenResult(
                        Token(TokenGroup.COMPARISONOPERATORS, TokenType.GREATERTHANEQUALS, ">="),
                        position
                    )
                else
                    TokenResult(
                        Token(TokenGroup.COMPARISONOPERATORS, TokenType.GREATERTHAN, ">"),
                        position
                    )
            with
            | :? System.IndexOutOfRangeException ->
                TokenResult(
                    Token(TokenGroup.COMPARISONOPERATORS, TokenType.GREATERTHAN, ">"),
                    position
                )
            | _ -> raise (System.Exception("Invalid greater than format"))

        member _.MakeLessThan(text:string, position:int) : TokenResult =
            let mutable position = position + 1
            try
                if text.[position] = '=' then
                    position <- position + 1
                    TokenResult(
                        Token(TokenGroup.COMPARISONOPERATORS, TokenType.LESSTHANEQUALS, "<="),
                        position
                    )
                else
                    TokenResult(
                        Token(TokenGroup.COMPARISONOPERATORS, TokenType.LESSTHAN, "<"),
                        position
                    )
            with
            | :? System.IndexOutOfRangeException ->
                TokenResult(
                    Token(TokenGroup.COMPARISONOPERATORS, TokenType.LESSTHAN, "<"),
                    position
                )
            | _ -> raise (System.Exception("Invalid less than format"))

        member _.MakeEquals(text:string, position:int) =
            let mutable position = position + 1
            try
                if text.[position] = '=' then
                    position <- position + 1
                    TokenResult(
                        Token(TokenGroup.COMPARISONOPERATORS, TokenType.EQUALS, "=="),
                        position
                    )
                else
                    TokenResult(
                        Token(TokenGroup.SYNTAX, TokenType.EQUALS, "="),
                        position
                    )
            with
            | :? System.IndexOutOfRangeException ->
                TokenResult(
                    Token(TokenGroup.SYNTAX, TokenType.EQUALS, "="),
                    position
                )
            | _ -> raise (System.Exception("Invalid equals format"))

        member _.MakePlus(text:string, position:int) =
            let mutable position = position + 1
            try
                if text.[position] = '=' then
                    position <- position + 1
                    TokenResult(
                        Token(TokenGroup.SYNTAX, TokenType.PLUSEQUALS, "+="),
                        position
                    )
                else
                    TokenResult(
                        Token(TokenGroup.OPERATORS, TokenType.PLUS, "+"),
                        position
                    )
            with
            | :? System.IndexOutOfRangeException ->
                TokenResult(
                    Token(TokenGroup.OPERATORS, TokenType.PLUS, "+"),
                    position
                )
            | _ -> raise (System.Exception("Invalid plus format"))

        member _.MakeDivide(text:string, position:int) =
            let mutable position = position + 1

            try 
                if text.[position] = '/' then
                    position <- position + 1
                    TokenResult(
                        Token(TokenGroup.SYNTAX, TokenType.DOUBLEDIVIDE, "//"),
                        position
                    )
                else
                    TokenResult(
                        Token(TokenGroup.OPERATORS, TokenType.DIVIDE, "/"),
                        position
                    )
            with
                | :? System.IndexOutOfRangeException ->
                    TokenResult(
                        Token(TokenGroup.OPERATORS, TokenType.DIVIDE, "/"),
                        position
                    )
                | _ -> raise (System.Exception("Invalid divide format"))