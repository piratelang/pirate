module Pirate.Lexer.Test.KeyWordServiceTest

open Xunit
open Pirate.Lexer
open Pirate.Lexer.Enums


[<Fact>]
let ShouldReturnVar () =
    Assert.Equal(
        VAR, 
        KeyWordService().GetTypeKeyword("var")
    )

[<Fact>]
let ShouldReturnInt () = 
    Assert.Equal(
        INT, 
        KeyWordService().GetTypeKeyword("int")
    )

[<Fact>]
let ShouldReturnFloat () = 
    Assert.Equal(
        FLOAT, 
        KeyWordService().GetTypeKeyword("float")
    )

[<Fact>]
let ShouldReturnString () = 
    Assert.Equal(
        STRING, 
        KeyWordService().GetTypeKeyword("string")
    )

[<Fact>]
let ShouldReturnChar () = 
    Assert.Equal(
        CHAR, 
        KeyWordService().GetTypeKeyword("char")
    )

[<Fact>]
let ShouldReturnIf () = 
    Assert.Equal(
        IF, 
        KeyWordService().GetTokenControlKeyword("if")
    )

[<Fact>]
let ShouldReturnElse () = 
    Assert.Equal(
        ELSE, 
        KeyWordService().GetTokenControlKeyword("else")
    )

[<Fact>]
let ShouldReturnFor () = 
    Assert.Equal(
        FOR, 
        KeyWordService().GetTokenControlKeyword("for")
    )

[<Fact>]
let ShouldReturnTo () = 
    Assert.Equal(
        TO, 
        KeyWordService().GetTokenControlKeyword("to")
    )

[<Fact>]
let ShouldReturnForeach () = 
    Assert.Equal(
        FOREACH, 
        KeyWordService().GetTokenControlKeyword("foreach")
    )

[<Fact>]
let ShouldReturnIn () = 
    Assert.Equal(
        IN, 
        KeyWordService().GetTokenControlKeyword("in")
    )

[<Fact>]
let ShouldReturnWhile () = 
    Assert.Equal(
        WHILE, 
        KeyWordService().GetTokenControlKeyword("while")
    )

[<Fact>]
let ShouldReturnFunc () = 
    Assert.Equal(
        FUNC, 
        KeyWordService().GetTokenControlKeyword("func")
    )

[<Fact>]
let ShouldReturnClass () = 
    Assert.Equal(
        CLASS, 
        KeyWordService().GetTokenControlKeyword("class")
    )

[<Fact>]
let ShouldReturnNew () = 
    Assert.Equal(
        NEW, 
        KeyWordService().GetTokenControlKeyword("new")
    )

[<Fact>]
let ShouldReturnVoid () = 
    Assert.Equal(
        VOID, 
        KeyWordService().GetTypeKeyword("void")
    )

[<Fact>]
let ShouldReturnReturn () = 
    Assert.Equal(
        RETURN, 
        KeyWordService().GetTokenControlKeyword("return")
    )
    