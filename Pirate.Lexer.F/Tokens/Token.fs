namespace Pirate.Lexer.F.Tokens

open Pirate.Lexer.F.Tokens.Enums

type Token(tokenGroup:TokenGroup, tokenType:TokenType, value:obj) =
    member _.TokenGroup = tokenGroup
    member _.TokenType = tokenType
    member _.Value = value

    member this.Matches (tokenType: obj, value: obj) : bool =
        if value = null || this.Value = null then
            if this.TokenType.Equals(tokenType) then true
            else false
        else
            if (this.TokenType.Equals(tokenType) && this.Value.Equals(value)) then true
            else false

    override this.ToString() = $"%s{string this.TokenGroup}:%s{string this.TokenType}:%A{this.Value}"

type TokenResult(token:Token, position:int) =
    member _.Token = token
    member _.Position = position