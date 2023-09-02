open Pirate.Common.Logger
open System
open Pirate.Lexer.F




// For more information see https://aka.ms/fsharp-console-apps
printfn "Hello from F#"

let logger = new Logger()

let input = Console.ReadLine()
let lexer = new Lexer(logger, new TokenRepository(new KeyWordService()))
let result = lexer.MakeTokens(input, "test")

for token in result do
    printfn "%A" (token.ToString())