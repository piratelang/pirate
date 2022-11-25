<p align="center">
    <img height="88" src=".github/owllogo.jpg" alt="Material Bread logo" style="margin-right:10px;">
    <img width="500" src=".github/logo.png" alt="Material Bread logo">
    <br>
    <a href="https://www.nuget.org/packages/PirateLang.CLI">
        <img src="https://img.shields.io/nuget/v/PirateLang.CLI.svg" alt="HTML tutorial">
    </a>
    <a href="https://marketplace.visualstudio.com/items?itemName=joerivanarkel.piratelang">
        <img src="https://img.shields.io/visual-studio-marketplace/v/joerivanarkel.piratelang?label=VSCode%20Extension" alt="HTML tutorial">
    </a>
    <a href="https://github.com/joerivanarkel/PirateLang/actions/workflows/dotnet.yml">
        <img src="https://github.com/joerivanarkel/PirateLang/actions/workflows/dotnet.yml/badge.svg" alt="HTML tutorial">
    </a>
    <a href="https://github.com/piratelang/PirateLang/releases">
        <img src="https://img.shields.io/github/v/release/joerivanarkel/piratelang" alt="HTML tutorial">
    </a>
</p>

# Pirate Programming Language


## Installation
Pirate comes completely installed in a dotnet tool. Sinmply install the NuGet package linked and use `pirate init` to create the first module. A VSCode extension is available for syntax highlighting [here](https://github.com/joerivanarkel/PirateLang.VSCode). Laso a standalone executable is availeble in the releases tab.

## Syntax and Structure
A simple Hello World in pirate looks like this:
```nim
IO.print("Hello World");
```
More syntax is defined in the [Syntax.md](syntax.md) file.


## Solution Structure
### PirateLexer
Takes a .pirate file and lexes it into a list of tokens. A single token has a group and type property.

### PirateParser
Takes a list of tokens from the lexer and parses it to a Scope. A scope consists of a list of Node. A node is created in the Parsers. This list of nodes is serialized locally.

### PirateInterpreter
Takes the serialized scope and visits each node for a result. Returns a `BaseValue` type object.

### Shell
Runs the Lexer, Parser and Interpreter of the file path in the argument.
