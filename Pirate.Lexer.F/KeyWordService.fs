namespace Pirate.Lexer

open Pirate.Lexer.Enums

type KeyWordService() =
    member _.typeKeyWords = ["var"; "int"; "float"; "string"; "char"; "void"]
    member _.controlKeyWords = [ "if"; "else"; "for"; "to"; "foreach"; "in"; "while"; "func"; "class"; "new"; "return" ]

    member this.GetTypeKeyword (idString:string) : TokenType =
        if this.typeKeyWords |> List.contains idString then
            match idString with
            | "var" -> VAR;
            | "int" -> INT;
            | "float" -> FLOAT;
            | "string" -> STRING;
            | "char" -> CHAR;
            | "void" -> VOID;
            | _ -> raise (System.NotImplementedException($"Type keyword, {idString} has not been implemented"));
        else
            TokenType.Empty;

    member this.GetTokenControlKeyword (idString:string) : TokenType =
        if this.controlKeyWords |> List.contains idString then
            match idString with
            | "if" -> IF;
            | "else" -> ELSE;
            | "for" -> FOR;
            | "to" -> TO;
            | "foreach" -> FOREACH;
            | "in" -> IN;
            | "while" -> WHILE;
            | "func" -> FUNC;
            | "class" -> CLASS;
            | "new" -> NEW;
            | "return" -> RETURN;
            | _ -> raise (System.NotImplementedException($"Control keyword, {idString} has not been implemented"));
        else
            TokenType.Empty;