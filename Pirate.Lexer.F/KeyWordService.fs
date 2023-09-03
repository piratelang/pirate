namespace Pirate.Lexer.F

open Pirate.Lexer.F.Tokens.Enums

type IKeyWordService =
    abstract member GetTypeKeyword : string -> TokenType
    abstract member GetTokenControlKeyword : string -> TokenType

type KeyWordService() =
    member _.typeKeyWords = ["var"; "int"; "float"; "string"; "char"; "void"]
    member _.controlKeyWords = [ "if"; "else"; "for"; "to"; "foreach"; "in"; "while"; "func"; "class"; "new"; "return" ]

    interface IKeyWordService with
        member this.GetTypeKeyword (idString:string) : TokenType =
            if this.typeKeyWords |> List.contains idString then
                match idString with
                | "var" -> TokenType.VAR;
                | "int" -> TokenType.INT;
                | "float" -> TokenType.FLOAT;
                | "string" -> TokenType.STRING;
                | "char" -> TokenType.CHAR;
                | "void" -> TokenType.VOID;
                | _ -> raise (System.NotImplementedException($"Type keyword, {idString} has not been implemented"));
            else
                TokenType.Empty;

        member this.GetTokenControlKeyword (idString:string) : TokenType =
            if this.controlKeyWords |> List.contains idString then
                match idString with
                | "if" -> TokenType.IF;
                | "else" -> TokenType.ELSE;
                | "for" -> TokenType.FOR;
                | "to" -> TokenType.TO;
                | "foreach" -> TokenType.FOREACH;
                | "in" -> TokenType.IN;
                | "while" -> TokenType.WHILE;
                | "func" -> TokenType.FUNC;
                | "class" -> TokenType.CLASS;
                | "new" -> TokenType.NEW;
                | "return" -> TokenType.RETURN;
                | _ -> raise (System.NotImplementedException($"Control keyword, {idString} has not been implemented"));
            else
                TokenType.Empty;