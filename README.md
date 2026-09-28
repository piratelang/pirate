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
        <img src="https://img.shields.io/nuget/v/PirateLang.CLI.svg" alt="PirateLang.CLI on NuGet">
    </a>
    <a href="https://marketplace.visualstudio.com/items?itemName=joerivanarkel.piratelang">
        <img src="https://img.shields.io/visual-studio-marketplace/v/joerivanarkel.piratelang?label=VSCode%20Extension" alt="VSCode Extension">
    </a>
    <a href="https://github.com/piratelang/PirateLang/releases">
        <img src="https://img.shields.io/github/v/release/joerivanarkel/piratelang" alt="Release">
    </a>
    <a href="https://wakatime.com/badge/user/261ee501-1b33-464c-8873-6be422308f2f/project/addb9833-5df4-46f5-98b2-36bfb78b5994">
        <img src="https://wakatime.com/badge/user/261ee501-1b33-464c-8873-6be422308f2f/project/addb9833-5df4-46f5-98b2-36bfb78b5994.svg" alt="wakatime">
    </a>
</p>

# Pirate Programming Language

Pirate is a toy programming language written in C# and F#, created to learn
more about programming languages and compilers.

**The current direction is v2** — a ground-up rewrite living in `src-v2/`
(this repository's dev branch). v1 — the shipped, dynamically-typed
tree-walking interpreter in `src/` — keeps working while v2 lands; the two
solutions share no projects.

## v2 in one screen

v2 replaces v1's interpreted pipeline with a compiled one:

```
source.pirate → lexer (C#) → parser (F#, Pratt) → static type-checker
              → bytecode compiler → stack VM          (compiler/VM: next)
```

- **Statically typed** — every type error is a compile-time diagnostic with
  a real source location; nothing crashes mid-run for a typo'd type.
- **Every error in one pass** — stages collect typed diagnostics instead of
  throwing; the Spectre.Console CLI renders them all, `LEX`/`SYN`/`SEM`
  coded, with source excerpts and carets.
- **Script-shaped programs** — the entry module's top-level statements *are*
  the program (`pirate run`), with functions for reusable code and a
  `.fleet` project manifest.

A hello world looks like this:

```pirate
import standard Terminal;

PrintLine("Hello World");
```

| Read this | For |
|---|---|
| [`docs/GRAMMAR.md`](docs/GRAMMAR.md) | the canonical v2 language specification |
| [`docs/examples/`](docs/examples/) | runnable syntax examples, pinned by tests |
| [`docs/architecture/`](docs/architecture/) | how v2 is built, and the honest current state |
| [`docs/CLI.md`](docs/CLI.md) / [`docs/FLEET.md`](docs/FLEET.md) | the `pirate` command surface / project manifests |
| [`docs/GRAMMAR_CHANGES.md`](docs/GRAMMAR_CHANGES.md) | why v2's language differs from v1's |

Status: the front end (lexer, parser, semantics, CLI `build`/`run` checking,
incremental content-hash cache) is complete and tested; the bytecode
compiler, VM, standard library, and module linking (`import module` /
`import external`) are the next milestones — see
[`docs/architecture/v2-architecture.md`](docs/architecture/v2-architecture.md#current-state).

Build and test v2 (requires .NET 9 SDK):

```
dotnet test src-v2/PirateLang.slnx
```

---

# v1 — shipped (`src/`)

The published implementation ([`PirateLang.CLI` on
NuGet](https://www.nuget.org/packages/PirateLang.CLI)): dynamically typed,
tree-walking interpreter. What v1 *actually does*, file by file, is
described in
[`docs/architecture/v1-architecture.md`](docs/architecture/v1-architecture.md);
its historical `GRAMMAR.md`/`SYNTAX.md` root docs drifted from the
implementation and were removed.

## Installation

TBD

## Syntax and Structure

A simple Hello World in pirate looks like this:

```nim
extern Standard.Terminal.Print;

Print("Hello World");
```

More syntax is defined in the [grammar](docs/GRAMMAR.md) — but note that
that file now governs **v2**; for v1's real behavior see the
[v1 architecture doc](docs/architecture/v1-architecture.md).

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
