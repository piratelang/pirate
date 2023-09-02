namespace Pirate.Lexer.F.Tokens

open Pirate.Lexer.F.Tokens.Enums

type Token(tokenGroup:TokenGroup, tokenType:TokenType, value:obj) =
    member _.TokenGroup = tokenGroup
    member _.TokenType = tokenType
    member _.Value = value

    member this.Matches (tokenType: obj) (value: obj option) =
        if value = None || value = None then
            this.TokenType.Equals(tokenType)
        else
            this.TokenType.Equals(tokenType) && this.Value.Equals(value.Value)

    override this.ToString() = sprintf "%s:%s:%A" (string this.TokenGroup) (string this.TokenType) this.Value

type TokenResult(token:Token, position:int) =
    member _.Token = token
    member _.Position = position