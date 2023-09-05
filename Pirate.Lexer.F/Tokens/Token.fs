namespace Pirate.Lexer.Tokens

open Pirate.Lexer.TokenType.Enums
open Pirate.Lexer.Enums
open Pirate.Lexer.TokenType
open Newtonsoft.Json

type Token (tokenGroup:Pirate.Lexer.TokenType.Enums.TokenGroup, tokenType:Pirate.Lexer.TokenType.Enums.TokenType, value:obj) =
    [<JsonConstructor>]
    new(tokenGroup:Pirate.Lexer.TokenType.Enums.TokenGroup, tokenType:Pirate.Lexer.TokenType.Enums.TokenType) = Token(tokenGroup, tokenType, null)

    new(tokenGroup:Pirate.Lexer.Enums.TokenGroup, tokenType:Pirate.Lexer.Enums.TokenType, value:obj) = Token(TokenTypeMapper.ConvertTokenGroup(tokenGroup), TokenTypeMapper.ConvertTokenType(tokenType), value)
    new(tokenGroup:Pirate.Lexer.Enums.TokenGroup, tokenType:Pirate.Lexer.Enums.TokenType) = Token(TokenTypeMapper.ConvertTokenGroup(tokenGroup), TokenTypeMapper.ConvertTokenType(tokenType), null)

    member val TokenGroup = tokenGroup with get, set
    member val TokenType = tokenType with get, set
    member val Value = value with get, set

    member this.Matches (tokenType: obj, value: obj) : bool =
        if value = null || this.Value = null then
            if this.TokenType.Equals(tokenType) then true
            else false
        else
            if (this.TokenType.Equals(tokenType) && this.Value.Equals(value)) then true
            else false

    member this.Matches (tokenType: obj) : bool =
        if this.TokenType.Equals(tokenType) then true
        else false

    override this.ToString() = $"%s{string this.TokenGroup}:%s{string this.TokenType}:%A{this.Value}"

type TokenResult(token:Token, position:int) =
    member _.Token = token
    member _.Position = position