[![NuGet](https://img.shields.io/nuget/v/PirateLang.CLI.svg)](https://www.nuget.org/packages/PirateLang.CLI)

# Pirate Programming Language

## Installation
Pirate comes completely installed in a dotnet tool. Sinmply install the NuGet package linked and use `pirate init` to create the first module. A VSCode extension is avaialble for syntax highlighting [here](https://github.com/joerivanarkel/PirateLang.VSCode).

## Syntax and Structure
A simple Hello World in pirate looks like this:
```pirate
func main()
{
  print("Hello World");
}
```
More syntax is defined in the [Syntax.md](syntax.md) file.


## Solution Structure
### PirateLexer
Takes a .pirate file and lexes it into a list of tokens
### PirateParser
Takes a list of tokens from the lexer and prases it to the python syntax
### PirateInterpreter
Runs a python engine and runtime in C# to run the python code from the Parser.

### Shell
Runs the Lexer, Parser and Interpreter of the file path in the argument.
