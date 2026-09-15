# v2 Architecture

Solution: [`src-v2/PirateLang.slnx`](../../src-v2/PirateLang.slnx). Ground-up
rewrite — not a port of v1's code — targeting three goals: a CLI on
Spectre.Console, a single-pass lexer and a precedence-driven parser (fixing
the concrete correctness/perf problems documented in
[`v1-architecture.md`](v1-architecture.md)), and a genuinely **compiled**
pipeline (AST → bytecode → stack VM) with **static typing** checked before a
program runs, instead of v1's dynamically-typed tree-walking interpreter.

Independent from v1: no shared projects, no cross-references between `src/`
and `src-v2/`. Both must build and run on their own for as long as v1 is
still shipped.

## Pipeline

```
source.pirate
     │  Pirate.Lexer
     ▼
Token[]  (with line/column per token)
     │  Pirate.Parser
     ▼
AST  (Pirate.Syntax records)          ──▶ diagnostics (parse errors, collected not thrown)
     │  Pirate.Semantics
     ▼
Checked AST  (names resolved, every expression's type known)
                                        ──▶ diagnostics (type errors, collected not thrown)
     │  Pirate.Compiler
     ▼
Bytecode chunk(s)  (Pirate.VM opcodes + constant pool)
     │  Pirate.VM
     ▼
Program output (stdout, exit code)
```

Every stage after the lexer can fail *without throwing* — parser and
semantic errors are collected into a diagnostics list with source location,
so `Pirate.Cli` can print every error found in one pass (via
Spectre.Console) instead of stopping at the first one, matching how real
compilers behave and fixing v1's throw-on-first-error `ParserException`
behavior.

## Project graph

```
Pirate.Syntax  ←  Pirate.Lexer  ←  Pirate.Parser  ←┐
       ↑                                            ├─ Pirate.Cli (Spectre.Console.Cli)
       └────────  Pirate.Semantics  ─────────────────┤
                                                      │
Pirate.VM  ←  Pirate.Compiler ───────────────────────┤
     ↑                                                │
     └── Pirate.StandardLibrary ──────────────────────┘
```

### Pirate.Syntax

Plain AST record types (`FunctionDecl`, `IfStmt`, `BinaryExpr`, `ForInStmt`,
…) shared by the parser (produces them), the semantics pass (annotates/
validates them), and the compiler (consumes them). Data only — no per-node
interface hierarchy like v1's `INode`/`I*Node`, no logic, so no dedicated
test project (see [`../TESTING.md`](../TESTING.md)).

### Pirate.Lexer

Single pass over `ReadOnlySpan<char>`. No upfront mutation of the source (v1
stripped newlines before lexing, destroying line info). Tokens are structs
carrying `TokenType`, a value span, line, and column, appended to a
pre-sized growable buffer — no O(n²) list-append like v1's F# lexer.

### Pirate.Parser

Recursive-descent for statements, **precedence climbing (Pratt parsing)**
for expressions — one operator-precedence table drives all binary/unary/
comparison parsing (see [`../GRAMMAR.md`](../GRAMMAR.md) §3.5), replacing
v1's one-hand-rolled-method-per-operator-class approach
(`BinaryOperationNode` vs. `ComparisonOperationNode` parsed by entirely
separate code paths). Produces diagnostics + a partial AST on error instead
of throwing.

### Pirate.Semantics

Name resolution (variable/function slots) and static type checking per
[`../GRAMMAR.md`](../GRAMMAR.md) §2 — this is the pass that turns a type
error into a compile-time diagnostic instead of v1's runtime crash mid-
execution. Also where "non-`void` function has no reachable `return`" and
similar structural checks live.

### Pirate.Compiler

Walks the checked AST once per function, emitting a flat instruction array +
constant pool (a "chunk", type owned by `Pirate.VM` since both the compiler
and the VM need to agree on its shape). Draft opcode set:

| Category | Opcodes |
|---|---|
| Constants/locals | `LOAD_CONST`, `LOAD_LOCAL`, `STORE_LOCAL`, `LOAD_GLOBAL`, `STORE_GLOBAL` |
| Arithmetic | `ADD`, `SUB`, `MUL`, `DIV`, `MOD`, `POW`, `NEG` |
| Comparison/logic | `EQ`, `NEQ`, `LT`, `LTE`, `GT`, `GTE`, `AND`, `OR`, `NOT` |
| Control flow | `JUMP`, `JUMP_IF_FALSE`, `LOOP` (back-edge) |
| Functions | `CALL`, `RET`, `CALL_NATIVE` (stdlib/extern) |
| Arrays | `NEW_ARRAY`, `INDEX_GET`, `INDEX_SET` |

Exact set will grow as constructs are implemented; treat this table as a
starting point to update alongside the compiler, not a frozen spec.

### Pirate.VM

The opcode/chunk type definitions plus the stack-machine interpreter loop
(`while` + `switch` on opcode, an operand stack, call frames for function
calls/returns). Value representation is a tagged-union **struct**
(`PirateValue { ValueKind Kind; double Number; object? Ref; }` or
equivalent) so `int`/`float`/`bool`/`char` never heap-allocate — this is the
direct fix for v1's `BaseValue` class-per-type hierarchy where every value,
including a single integer, was a heap object with virtual dispatch.
Strings/arrays/function closures live behind `Ref`.

### Pirate.StandardLibrary

Native functions (`Standard.Terminal.Print`, `Standard.String.Length`, …)
registered into a table the VM's `CALL_NATIVE` opcode indexes into,
resolved by an `extern Standard.X.Y;` declaration — same dotted-namespace
convention as v1's `Pirate.Interpreter.StandarLibrary`, reimplemented against
the VM's calling convention instead of wrapping `BaseValue` objects.

### Pirate.Cli

`Spectre.Console.Cli` `CommandApp` with one `Command<TSettings>` per verb
(`run`, `build`, `new`, `init`, `shell` — mirroring v1's command surface),
replacing v1's hand-rolled `CommandManager`/`CommandFactory`/`ICommand`
dispatch and manual `-h`/`--help` handling. Diagnostics render as
`AnsiConsole` tables (file/line/col/message); build/run wrap in a
`Status`/spinner.

## Testing

See [`../TESTING.md`](../TESTING.md) for the full strategy: xUnit unit tests
per project above (`Pirate.Lexer.Test`, `Pirate.Parser.Test`,
`Pirate.Semantics.Test`, `Pirate.Compiler.Test`, `Pirate.VM.Test`,
`Pirate.StandardLibrary.Test`), plus `Pirate.Spec.Test` (Reqnroll/Gherkin)
running real `.pirate` programs end-to-end and asserting actual stdout —
unlike v1's equivalent suite, whose `Then` step never asserted anything.

## Current state

Scaffolded only: `src-v2/PirateLang.slnx` and all projects above exist, wired
together with the correct project references and package references
(`Spectre.Console.Cli` on `Pirate.Cli`; `Reqnroll.xUnit` + `FluentAssertions`
on `Pirate.Spec.Test`; `xunit` + `AutoFixture` + `FakeItEasy` on the other
`*.Test` projects), and the whole solution builds. No lexer, parser,
semantics, compiler, VM, or stdlib logic has been written yet — every
library project is presently an empty stub.
