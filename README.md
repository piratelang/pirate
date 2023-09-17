<p align="center">
    <img height="88" src=".github/owllogowhite.png" alt="Material Bread logo" style="margin-right:12px;"><br>
    <img width="500" src=".github/logo.png" alt="Material Bread logo">
    <br>
    <a href="https://github.com/joerivanarkel/PirateLang/actions/workflows/dotnet.yml">
        <img src="https://github.com/joerivanarkel/PirateLang/actions/workflows/dotnet.yml/badge.svg" alt=".NET">
    </a>
    <a href="https://github.com/piratelang/pirate/actions/workflows/github-code-scanning/codeql">
        <img src="https://github.com/piratelang/pirate/actions/workflows/github-code-scanning/codeql/badge.svg" alt="CodeQL">
    </a>
    <a href="https://www.nuget.org/packages/PirateLang.CLI">
        <img src="https://img.shields.io/nuget/v/PirateLang.CLI.svg" alt="HTML tutorial">
    </a>
    <a href="https://marketplace.visualstudio.com/items?itemName=joerivanarkel.piratelang">
        <img src="https://img.shields.io/visual-studio-marketplace/v/joerivanarkel.piratelang?label=VSCode%20Extension" alt="HTML tutorial">
    </a>
    <a href="https://github.com/piratelang/PirateLang/releases">
        <img src="https://img.shields.io/github/v/release/joerivanarkel/piratelang" alt="Release">
    </a>
    <a href="https://wakatime.com/badge/user/261ee501-1b33-464c-8873-6be422308f2f/project/addb9833-5df4-46f5-98b2-36bfb78b5994">
        <img src="https://wakatime.com/badge/user/261ee501-1b33-464c-8873-6be422308f2f/project/addb9833-5df4-46f5-98b2-36bfb78b5994.svg" alt="wakatime">
    </a>
</p>

# Pirate Programming Language

Pirate is a toy programming language that is written in C# and F#. It is a simple language that was created to learn more about programming languages and interpreters.

## Contents

- [Installation](#installation)
- [Syntax and Structure](#syntax-and-structure)
- [Solution Structure](#solution-structure)
  - [Pirate.Lexer](#piratelexer)
    - [Pirate.Lexer.Enums](#piratelexerenums)
    - [Pirate.Lexer.TokenType](#piratelexertokentype)
  - [Pirate.Parser](#pirateparser)
  - [Pirate.Interpreter](#pirateinterpreter)
  - [Shell](#shell)

## Installation

TBD

## Syntax and Structure

A simple Hello World in pirate looks like this:

```nim
IO.print("Hello World");
```

More syntax is defined in the [Syntax.md](syntax.md) file.

## Solution Structure

### Pirate.Lexer

Takes the input from a `.pirate` file and lexes it into a list of tokens.

The Lexer is written in F#, and consists of a Lexer, TokenRepository and KeyWordService. The Lexer takes the input and creates tokens for the character, yet when the Lexer encounters a larger token, i.e. a Identifier or a String, it will call the TokenRepository to create a more complex token. The TokenRepository will then call the KeyWordService to check if the token is a keyword. If it is, it will return the exact TokenType, if not, it will return a TokenType.Empty.

A token has a TokenGroup, a TokenType and a value. The TokenGroup is used to find groups of tokens that are related. The TokenType is used to find the exact type of token. The value is used to store the value of the token. For example, a string token has a value of "Hello World", as that is the exact value of the string.

##### Pirate.Lexer.Enums

Used to store the F# enums that are used in the Lexer.

##### Pirate.Lexer.TokenType

Used to store the C# enums that are used in the Parser and Interpreter. Also used to store the Mapper, which is used to map the F# enums to the C# enums.

### Pirate.Parser

Takes a list of tokens from the lexer and parses it to a Scope.

The Parser is written in C#, and consists of a Parser, many individual Parsers, a ParserFactory and a Scope. The Parser takes the list of tokens and finds the acompanying parser for the token. The parser will then parse the token and return a Node. A Parser itself may call other parsers to parse the token or the ParserFactory to create a new parser. The ParserFactory is used to create a new parser for a token. The Scope is used to store the nodes.

A Node is a representation of one or more tokens. It represents a expression, operation or value. With the way that parser works, it is possible to create a tree of nodes. For example, the expression `1 + 2 + 3` will be parsed to a tree of nodes, where the root node is the first `+`, which contains a left node of `1` and a right node of `2 + 3`. The `2 + 3` node will then have a left node of `2` and a right node of `3`. 

A scope consists of a list of Node. A node is created in the Parsers.

### Pirate.Interpreter

Takes the serialized scope and visits each node for a result. Returns a `BaseValue` type object.

### Shell

Runs the Lexer, Parser and Interpreter of the file path in the argument.
