# v1 Architecture

Solution: [`src/PirateLang.sln`](../../src/PirateLang.sln). Shipped
implementation (`PirateLang.CLI` on NuGet). Tree-walking interpreter,
dynamically typed, mixed F#/C#. This document describes what v1 **actually
does**, not what its own docs (`GRAMMAR.md`/`SYNTAX.md` at repo root) claim —
those had drifted from the implementation (see
[`../GRAMMAR_CHANGES.md`](../GRAMMAR_CHANGES.md) for the specific gaps).

## Pipeline

```
source.pirate
     │  Pirate.Lexer.F  (F#)
     ▼
List<Token>
     │  Pirate.Parser  (C#)
     ▼
Scope (List<INode>)
     │  Pirate.Interpreter  (C#)
     ▼
List<BaseValue>  (program result / side effects via stdout)
```

### Lexer — `Pirate.Lexer.F`, `Pirate.Lexer.Enums`, `Pirate.Lexer.TokenType`

F# project. [`Lexer.fs`](../../src/Pirate.Lexer.F/Lexer.fs) does a
character-by-character scan of the whole source string, dispatching complex
tokens (numbers, identifiers/keywords, strings, chars, multi-char operators)
to [`TokenRepository.fs`](../../src/Pirate.Lexer.F/TokenRepository.fs), which
in turn asks [`KeyWordService.fs`](../../src/Pirate.Lexer.F/KeyWordService.fs)
whether an identifier is actually a keyword.

Two known problems, not just style issues:
- `tokens <- tokens @ [token]` appends to an **F# immutable list** per token
  — O(n) per append, O(n²) total for a file of n tokens.
- The input string has `\n`, `\r`, `\t`, and four-space runs stripped
  **before** lexing (`text.Replace("\n","")...`), which destroys line
  information; there is no line/column on tokens, so errors can't point at a
  location.

`Pirate.Lexer.Enums` holds the F# `TokenType`/`TokenGroup` discriminated
unions. `Pirate.Lexer.TokenType` holds a parallel **C#** enum plus a
`TokenTypeMapper` — this exists purely to bridge F#'s enum representation
into something the C# parser/interpreter can consume.

Keywords already reserved in the lexer but with no parser/interpreter
support: `foreach`, `class`, `new` (see `KeyWordService.fs`).

### Parser — `Pirate.Parser`

C#. [`Parser.cs`](../../src/Pirate.Parser/Parser.cs) drives a
[`ParserFactory`](../../src/Pirate.Parser/Parsers/ParserFactory.cs) that
picks a per-construct parser from
[`Parsers/`](../../src/Pirate.Parser/Parsers) (one class per statement kind:
`IfStatementParser`, `WhileLoopStatementParser`, `ForLoopStatementParser`,
`FunctionDeclartionParser`, `ExternParser`, `VariableDeclarationParser`,
`OperationParser`, `IdentifierParser`, `CommentParser`). Each parser hand-rolls
its own recursive descent and precedence (there is no single precedence
table — see `OperationParser`/`BinaryOperationNode`/`ComparisonOperationNode`
for how binary vs. comparison operators are split).

Output is a `Scope` — a flat `List<INode>` — where each
[`Node/`](../../src/Pirate.Parser/Node) type is a class implementing its own
`I*Node` interface (one interface per node type, see
[`Node/Interfaces/`](../../src/Pirate.Parser/Node/Interfaces)) and its own
`IsValid()` self-check. There is no shared AST base beyond `INode`, no
visitor pattern — the interpreter re-derives node kind via pattern
matching/casts.

Parse errors throw a `ParserException` immediately (see
[`Pirate.Common.Exception/Exceptions/ParserException.cs`](../../src/Pirate.Common.Exception/Exceptions/ParserException.cs))
— the first error aborts parsing; there is no error recovery or multi-error
reporting.

### Interpreter — `Pirate.Interpreter`, `Pirate.Interpreter.Runtime`, `Pirate.Interpreter.StandarLibrary`

C#. [`Interpreter.cs`](../../src/Pirate.Interpreter/Interpreter.cs) walks the
`Scope`'s nodes; an
[`InterpreterFactory`](../../src/Pirate.Interpreter/Interpreters/InterpreterFactory.cs)
picks a per-node-type interpreter from
[`Interpreters/`](../../src/Pirate.Interpreter/Interpreters) (mirrors the
parser's one-class-per-construct shape: `IfStatementInterpreter`,
`WhileLoopStatementInterpreter`, `BinaryOperationInterpreter`, etc.), each
evaluating its node directly against a
[`ValueTable`](../../src/Pirate.Interpreter.Runtime/ValueTable.cs) (variable
storage) held in a singleton
[`Runtime`](../../src/Pirate.Interpreter.Runtime/Runtime.cs). There is no
separate compile step and no bytecode — every loop iteration re-walks the AST.

Values are a class hierarchy under
[`Values/`](../../src/Pirate.Interpreter/Values):
`BaseValue` → `IntegerValue` / `FloatValue` / `StringValue` / `CharValue` /
`BooleanValue` / `FunctionValue` (with a `CSharpFunction` subtype for native
functions). Every value is a heap-allocated object with virtual dispatch,
even for a single `int`. Typing is fully dynamic — there is no check that
runs before execution; a type mismatch surfaces as a runtime exception mid-run.

`Pirate.Interpreter.StandarLibrary` registers native functions (note the
project's own typo: "StandarLibrary") organized by dotted namespace under
[`Standard/`](../../src/Pirate.Interpreter.StandarLibrary/Standard)
(`Standard.Terminal.Print/PrintLine/Read`, `Standard.String.Length/CharAt/
Concat/Split/IndexOf/LastIndexOf/CharCodeAt`), resolved by an `extern
Standard.X.Y;` declaration ([`ExternNode`](../../src/Pirate.Parser/Node/ExternNode.cs) →
[`ExternInterpreter`](../../src/Pirate.Interpreter/Interpreters/ExternInterpreter.cs)).

### Shell — `Shell`, `Shell.ModuleList`, `Shell.Project`

C#, `net6.0`, packaged as `PirateLang.CLI`.
[`Program.cs`](../../src/Shell/Program.cs) hand-wires a
`Microsoft.Extensions.DependencyInjection` container and hands off to
[`Application.cs`](../../src/Shell/Application.cs), which either prints
no-args help ([`NoCommand`](../../src/Shell/Commands/NoCommand.cs)) or asks a
hand-rolled [`CommandManager`](../../src/Shell/CommandManager.cs) /
[`CommandFactory`](../../src/Shell/Commands/CommandFactory.cs) to dispatch
`args[0]` to one of `build`/`init`/`new`/`run`/`shell`
([`Commands/`](../../src/Shell/Commands)) — each command parses its own
remaining args manually and implements its own `Help()`. `-h`/`--help`
handling is a manual `args.Contains(...)` check in `CommandManager`, not
generated. Output is raw `Console.Write*` with manual `ConsoleColor` swaps.

### Common libraries

- `Pirate.Common` — `EnvironmentVariables`, `ObjectSerializer`, and the
  original (now largely superseded by `Pirate.Common.Exception`) `Errors/`
  exception types.
- `Pirate.Common.Exception` — the exception hierarchy actually referenced
  elsewhere (`ParserException`, `RuntimeCommandException`,
  `TypeConversionException`, `InvalidSyntaxException`, `FileException`) plus
  resource-based `ExceptionMessages`.
- `Pirate.Common.Logger` — a small custom `ILogger`, not
  `Microsoft.Extensions.Logging`; console/file sinks selected by
  `UseConsoleEnum`/`UseFileEnum`.
- `Pirate.Common.FileHandler` — `.pirate` file read/write helpers.

### Tests

`Pirate.Lexer.F.Test`, `Pirate.Parser.Test`, `Pirate.Interpreter.Test`,
`Pirate.Common.Test` — xUnit + AutoFixture + FakeItEasy unit tests per
project. `Pirate.Spec.Test` — SpecFlow.xUnit (EOL; v2 uses its successor,
Reqnroll) Gherkin end-to-end tests under
[`Features/`](../../src/Pirate.Spec.Test/Features), running real `.pirate`
snippets through the actual `Shell` `run` command. **Known gap:** the `Then`
step in
[`CommonSteps.cs`](../../src/Pirate.Spec.Test/StepDefinitions/CommonSteps.cs)
only prints the result to the console — it never calls `.Should()` — so
these scenarios cannot fail regardless of actual output. Don't treat a green
`Pirate.Spec.Test` run as v1 behavior verification; read the actual node/
interpreter code instead.
