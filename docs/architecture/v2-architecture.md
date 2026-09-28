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
Token[]  (with line/column per token)          ──▶ LexError[] (collected, not thrown)
      │  Pirate.Parser
      ▼
AST  (Pirate.Syntax records)                   ──▶ SyntaxError[] (collected, not thrown)
      │  Pirate.Semantics
      ▼
Checked AST  (names resolved, every expression's type known)
                                                 ──▶ SemanticsError[] (collected, not thrown)
      │  Pirate.Compiler
      ▼
Instructions + ConstantPool  (Pirate.VM bytecode)
      │  Pirate.VM
      ▼
Program output (stdout, exit code)
```

Every stage after the lexer can fail *without throwing* — errors are
collected into a typed list with source location, so `Pirate.Cli` can print
every error found in one pass (via Spectre.Console) instead of stopping at
the first one, matching how real compilers behave and fixing v1's
throw-on-first-error `ParserException` behavior.

## Project graph

```
Pirate.Syntax  ←  Pirate.Lexer ✅  ←  Pirate.Parser ✅  ←┐
        ↑                                                 ├─ Pirate.Cli (Spectre.Console.Cli)
        └── Pirate.Semantics ✅ ──────────────────────────┤
                                                           │
Pirate.VM  ←  Pirate.Compiler ────────────────────────────┤
      ↑                                                    │
      └── Pirate.StandardLibrary ──────────────────────────┤
                                                           │
                  Pirate.Shared.File ← Pirate.Fleet ───────┤
                  Pirate.Shared.Logging ───────────────────┘
```

### Pirate.Syntax

Plain AST record types (`VariableDeclarationNode`, `IfStatementNode`,
`FunctionDeclarationNode`, `BinaryOperationNode`, …) shared by the parser
(produces them), the semantics pass (annotates/validates them), and the
compiler (consumes them). Data only — no per-node interface hierarchy like
v1's `INode`/`I*Node`, no logic, so no dedicated test project (see
[`../TESTING.md`](../TESTING.md)).

Also defines the error type hierarchy:
- `CompilationError` (abstract base class with `Message`, `StartLocation`, `EndLocation?`)
- `LexError` (`LexErrorKind` — 7 values)
- `SyntaxError` (`SyntaxErrorKind` — 37 values)
- `SemanticsError` (`SemanticsErrorKind` — 14 values)

Error codes (`LEX-xxx`, `SYN-xxx`, `SEM-xxx`) are **not** part of this
project. They are assigned by `Pirate.Cli`'s `ErrorMapper` based on the
`*ErrorKind` enum value — the compiler libraries are code-agnostic.

`Pirate.Syntax` also holds the semantic model's *data* types — `PirateType`
(a location-free scalar-plus-array type value) and the `Symbol` family
(`VariableSymbol`, `FunctionSymbol`, `BuiltinSymbol`, `SymbolScope`) under
`Symbols/` — because the checked AST annotates nodes with them and both the
semantics pass and (later) the compiler must agree on their shape. The
resolution *machinery* (`SymbolTable`, the analyzer) lives in
`Pirate.Semantics` — see [v2-012] and [v2-022] in
[`../design/DESIGN_DECISIONS.md`](../design/DESIGN_DECISIONS.md).

### Pirate.Lexer

Single pass over `ReadOnlySpan<char>`. No upfront mutation of the source (v1
stripped newlines before lexing, destroying line info). Tokens are structs
carrying `TokenType`, a value span, line, and column, appended to a
pre-sized growable buffer — no O(n²) list-append like v1's F# lexer.
Written in C#, already complete and tested (372 lines, 83 tests).

Errors are emitted as `LexError` with a `LexErrorKind` enum value
(`UnexpectedCharacter`, `UnterminatedStringLiteral`, `IntegerOutOfRange`,
etc.), collected in `LexResult.Errors`. No codes, no throwing.

### Pirate.Parser

Recursive-descent for statements, **precedence climbing (Pratt parsing)**
for expressions — one operator-precedence table drives all binary/unary/
comparison parsing (see [`../GRAMMAR.md`](../GRAMMAR.md) §3.5), replacing
v1's one-hand-rolled-method-per-operator-class approach
(`BinaryOperationNode` vs. `ComparisonOperationNode` parsed by entirely
separate code paths). Produces `SyntaxError` list + a partial AST on error
instead of throwing. Written in F# — Pratt parsing maps naturally to F#
pattern matching and discriminated unions.

### Pirate.Semantics

Name resolution (variable/function slots) and static type checking per
[`../GRAMMAR.md`](../GRAMMAR.md) §2 — this is the pass that turns a type
error into a compile-time diagnostic instead of v1's runtime crash mid-
execution. Also where "non-`void` function has no reachable `return`" and
similar structural checks live.

**Symbol Table** — resolves all identifier names at compile time, replacing
runtime string lookups with numeric indices:

```
SymbolTable { parent?: SymbolTable, symbols: Dictionary<string, Symbol>, scopeLevel: int }
Symbol      { Name: string, Scope: SymbolScope, Index: int }
```

Scope tiers (matching Thorsten Ball's compiler, extended for Pirate's `extern`):

| Scope | Purpose | Example |
|-------|---------|---------|
| `GlobalScope` | Module-level variables and functions | top-level `var x = 5` |
| `LocalScope` | Function parameters and local variables | `func f(int x)` |
| `BuiltinScope` | Resolved standard-library functions (`extern`) | `extern Standard.Terminal.Print` |
| `FreeScope` | Outer-scope variables captured by closures (future) | nested function referencing outer `x` |

The semantics pass walks the AST, builds a `SymbolTable` per scope, resolves
every identifier to a `Symbol`, and annotates nodes with their resolved
symbols and inferred types. The compiler consumes these resolved symbols to
emit index-based opcodes (`LOAD_GLOBAL 3`, `LOAD_LOCAL 0`) instead of
name-based lookups.

Implemented shape (`Pirate.Semantics`, C#): `ISemanticAnalyzer` /
`SemanticAnalyzer` with `AddPirateSemantics()` DI registration, producing a
`SemanticResult` (checked AST + collected `SemanticsError`s — the same
collect-don't-throw result shape as `LexResult` / `ParseResult`). Two passes
over a module: first the top-level definitions (externs resolved against
`BuiltinRegistry`, `import standard <NS>` binding each group member by both
leaf name and dotted path, function signatures hoisted so recursion and
forward calls are legal), then the module's top-level statements in global
scope, then every function body in its own scope. `import module` /
`import external` parse but are reported as `SEM-013` until the module
linker lands. `BuiltinRegistry` is the single source of builtin signatures;
when `Pirate.StandardLibrary` lands it registers implementations against the
same names/indexes. Node annotation happens through nullable mutable
properties on the Syntax records (`InferredType`, `ResolvedSymbol`,
`LoopVariable`, `ResolvedCallee`) — the same tree is the checked AST.

### Pirate.Compiler

Walks the checked AST once per function, emitting a flat instruction byte
array + constant pool (a "chunk", type owned by `Pirate.VM` since both the
compiler and the VM need to agree on its shape).

**Bytecode format** — flat `byte[]` wrapped in an `Instructions` struct; the
authoritative opcode table and `Instructions` API are in
[## Bytecode Format](#bytecode-format) below. Opcode numbering is still
unfixed until compiler/VM phase 1 lands — do not duplicate the table here.

**Constant pool** — `PirateValue[]` (tagged-union struct array). All
compile-time-evaluable literals are stored here and referenced by index,
keeping bytecode compact. The pool deduplicates: `Add()` returns the index
of an existing value if already present.

**Backpatching** — `emit(opcode, operands...)` appends to the instruction
byte array and returns the position before the append. This enables patching
forward jump offsets (for `if`, `while`, `for`) after the target is known:

```csharp
var jumpNotTruthy = compiler.Emit(OpJumpNotTruthy, 0xFFFF); // placeholder
compiler.Compile(consequence);
var afterConsequence = compiler.Emit(OpJump, 0xFFFF);
compiler.PatchJump(jumpNotTruthy, compiler.Position());
compiler.Compile(alternative);
compiler.PatchJump(afterConsequence, compiler.Position());
```

**Interleaved growth** — compiler and VM grow together, not sequentially.
Start with 1-2 opcodes + stub VM, then expand both in lockstep (see
"Current state" below for the growth plan). This avoids speculating about
bytecode before you know how the VM will execute it.

### Pirate.VM

The opcode/chunk type definitions plus the stack-machine interpreter loop
(`while` + `switch` on opcode, an operand stack, call frames for function
calls/returns). Value representation is a tagged-union **struct**
(`PirateValue { ValueKind Kind; double Number; object? Ref; }` or
equivalent) so `int`/`float`/`bool`/`char` never heap-allocate — this is the
direct fix for v1's `BaseValue` class-per-type hierarchy where every value,
including a single integer, was a heap object with virtual dispatch.
Strings/arrays/function closures live behind `Ref`.

**Single stack + call frames** — rather than separate call-stack and
data-stack, the VM uses call frames that define regions of a single
pre-allocated stack:

```csharp
struct CallFrame {
    CompiledFunction Function;
    int InstructionPointer;
    int BasePointer; // where this frame's locals start on the stack
}
// Stack: [args...][locals...][temporaries...]
//        ^-- basePointer
// Locals: stack[basePointer + localIndex]
```

### Pirate.StandardLibrary

Native functions (`Standard.Terminal.Print`, `Standard.String.Length`, …)
registered into a table the VM's `CALL_BUILTIN` opcode indexes into,
resolved by an `extern Standard.X.Y;` declaration — same dotted-namespace
convention as v1's `Pirate.Interpreter.StandarLibrary`, reimplemented against
the VM's calling convention instead of wrapping `BaseValue` objects.

### Pirate.Shared.File

Dependency-free file-path helpers for both `.pirate` and `.fleet` files —
kept in one project since, past the two generic mechanisms both rely on,
they're the same kind of thing (find-by-extension, resolve-a-name-with-a-
default) and there's no reason a `.fleet`-aware component should need a
different project reference than a `.pirate`-aware one.

- `FileDiscovery.Discover(directory, extension, searchOption)` and
  `FileNameResolver.Resolve(argument, defaultName, extension)` are the
  actual `Directory.GetFiles`/default-and-strip-extension mechanisms,
  extension-agnostic.
- `PirateFileLocator` (recursive `*.pirate` discovery) and `PirateFileName`
  (filename argument → module name, defaulting to `"main"`) are thin
  wrappers supplying `.pirate`/`"main"`/recursive.
- `FleetFileLocator` (non-recursive `*.fleet` discovery — a manifest
  identifies a project root, so unlike a `.pirate` module it isn't
  searched for in subdirectories) and `FleetFileName` (`-n|--name`
  argument → manifest base name, defaulting to `"module"`) are the same
  wrapper shape, supplying `.fleet`/`"module"`/non-recursive. See
  [`../FLEET.md`](../FLEET.md) for how `Pirate.Fleet` uses them.

Split out of `Pirate.Cli` into its own project specifically so this logic
isn't tied to the CLI — a future `build`/`run` pipeline component other
than `Pirate.Cli` could depend on it without also pulling in
`Spectre.Console.Cli`. No package references, just the BCL.

One naming gotcha worth knowing if this project grows: any type declared in
a namespace nested under `Pirate.Shared.File` (e.g. its own test project,
`Pirate.Shared.File.Test`) has `System.IO.File` shadowed — an unqualified
`File` there resolves to the `Pirate.Shared.File` namespace itself, not the
BCL type, because C# namespace lookup prefers an enclosing namespace segment
over anything brought in by `using`. Code in that situation needs
`System.IO.File` written out in full — every test file here that touches
the filesystem does this.

### Pirate.Fleet

The `.fleet` project manifest — a `package.json`-style JSON file
(`FleetFile`: `name`, `version`, `entryPoint`, plus reserved empty-shaped
`build`/`dependencies` placeholders with no consumer yet) identifying a
directory as a Pirate project. See [`../FLEET.md`](../FLEET.md) for the
full schema. `FleetFileRepository` handles read/write on top of
`Pirate.Shared.File`'s `FleetFileLocator`; `FleetEntryPoint.Resolve(argument, directory)`
is the composition helper `Pirate.Cli` calls — explicit argument wins,
else the `*.fleet` `FleetFileLocator` finds in `directory` supplies
`entryPoint`, else `PirateFileName`'s default (`"main"`) applies. Discovery
and naming (`FleetFileLocator`/`FleetFileName`) live in `Pirate.Shared.File`
rather than here — once they were thin wrappers over that project's generic
`FileDiscovery`/`FileNameResolver`, there was nothing `.fleet`-specific
left in them worth a separate home from their `.pirate` counterparts.
What's left in `Pirate.Fleet` (the model, the JSON read/write, the
entry-point composition) is manifest-specific in a way file discovery and
naming aren't.

### Pirate.Cli

`Spectre.Console.Cli` `CommandApp` with one `Command<TSettings>` per verb
(`run`, `build`, `new`, `init`, `shell` — mirroring v1's command surface),
replacing v1's hand-rolled `CommandManager`/`CommandFactory`/`ICommand`
dispatch and manual `-h`/`--help` handling. `Program.cs` composes the app
through a `ServiceCollection` + Spectre `TypeRegistrar`/`TypeAdapter`, so
commands receive dependencies by constructor injection; the frontend
(`Services/CompilationPipeline` → `FrontendResult`) runs lexer → parser →
semantics and concatenates stage errors for one-pass rendering. Errors render as Spectre
tables with source excerpts (file/line/col/message/caret); build/run wrap
in a `Status`/spinner. File discovery/resolution is delegated to
`Pirate.Shared.File`, and `run`'s no-argument entry-point resolution to
`Pirate.Fleet`, rather than living in `Pirate.Cli` itself. The run-entry
rule (GRAMMAR.md §3.1) is a CLI concern, not a semantic one: `Services/
EntryPoint.HasRunnableBody` requires the entry module to carry top-level
statements — helper modules may declare without running, and a module with
no statements is "nothing to run" (formalized as RTN-004 when the VM
lands).

**Build cache** — content-hash based (`SHA256`) stored in `.pirate/cache.json`
(project-directory-relative keys; a corrupt/unreadable cache file is treated as
empty rather than fatal, and saves are temp-file + rename so a crash can't
write a half-cache).
Only modules whose file contents have changed are rebuilt, catching even
manual reverts that would fool `LastWriteTime`.

Another naming gotcha of the same kind as `Pirate.Shared.File`/`System.IO.File`
(see above): inside `Pirate.Cli.Commands`, the simple names `Lexer` and
`Parser` bind to the *namespaces* `Pirate.Lexer`/`Pirate.Parser` (the enclosing
`Pirate` namespace is searched before file-level `using`s resolve them), so
call sites fully qualify `Pirate.Lexer.Lexer.Tokenize(...)` and
`Pirate.Parser.Parser.Parse(...)`. `Pirate.Parser` itself is an F# project —
`module Parser` compiles to the static class `Pirate.Parser.Parser`.

**Error rendering** — `DiagnosticRenderer` reads the source file to produce
formatted output with line context and caret highlighting:

```
main.pirate:4:12 Expected ';' after expression *SYN-005*
  3 | func main() : void {
  4 |     var x = 5
    |              ^
  5 | }
```

## Bytecode Format

Bytecode is a flat byte array wrapped in an `Instructions` struct. Each
instruction is a 1-byte opcode followed by zero or more operands encoded as
big-endian integers. Operand widths are defined per-opcode in
`OpcodeDefinition`:

```
OpConstant:      0x00 + 2 bytes (constant pool index)
OpAdd:           0x01
OpSub:           0x02
OpMul:           0x03
OpDiv:           0x04
OpJumpNotTruthy: 0x05 + 2 bytes (forward jump offset)
OpJump:          0x06 + 2 bytes (forward jump offset)
LOAD_GLOBAL:     0x07 + 2 bytes (global index)
STORE_GLOBAL:    0x08 + 2 bytes (global index)
LOAD_LOCAL:      0x09 + 1 byte (local index)
STORE_LOCAL:     0x0A + 1 byte (local index)
CALL:            0x0B + 1 byte (argument count)
RETURN:          0x0C
RETURN_VALUE:    0x0D
CALL_BUILTIN:    0x0E + 2 bytes (builtin index)
```

The `Instructions` struct provides:
- `AsSpan()` — zero-allocation `ReadOnlySpan<byte>` for parsing
- `GetOperandAt(offset)` — decode big-endian operands
- `ToString()` — disassembly output (`"0000 OpConstant 1"`)

## Compiler/VM Growth Plan

Compiler and VM grow **together**, not sequentially. Each phase adds opcodes
to both sides at once:

| Phase | Compiler Emits | VM Executes |
|-------|---------------|-------------|
| 1 | `OpConstant` (integer literals) | Push constant onto stack |
| 2 | `OpAdd`, `OpSub`, `OpMul`, `OpDiv` | Pop two, compute, push |
| 3 | `OpTrue`, `OpFalse`, comparison ops | Boolean/comparison |
| 4 | `OpJumpNotTruthy`, `OpJump` (backpatched) | Instruction pointer manipulation |
| 5 | `LOAD_GLOBAL`, `STORE_GLOBAL` (symbol table) | Globals array by index |
| 6 | Strings, arrays, `INDEX_GET`, `INDEX_SET` | String/array objects |
| 7 | `CALL`, `RETURN_VALUE`, `LOAD_LOCAL`, `STORE_LOCAL` | Call frames on single stack |
| 8 | `CALL_BUILTIN` (stdlib) | Direct C# function call |
| 9 | `OpClosure`, free variable resolution | Closure objects with captured values |

## Testing

See [`../TESTING.md`](../TESTING.md) for the full strategy: xUnit unit tests
per project above (`Pirate.Lexer.Test`, `Pirate.Parser.Test`,
`Pirate.Semantics.Test`, `Pirate.Compiler.Test`, `Pirate.VM.Test`,
`Pirate.StandardLibrary.Test`), plus `Pirate.Spec.Test` (Reqnroll/Gherkin)
running real `.pirate` programs end-to-end and asserting actual stdout —
unlike v1's equivalent suite, whose `Then` step never asserted anything.

## Error Code System

Errors are categorized by pipeline stage with a 3-letter prefix and
sequential number:

| Prefix | Stage | Range | File |
|--------|-------|-------|------|
| `LEX` | Lexer | `LEX-001`–`LEX-007` | `docs/errors/LEX_ERRORS.md` |
| `SYN` | Parser | `SYN-001`–`SYN-048` (no `SYN-040`) | `docs/errors/SYN_ERRORS.md` |
| `SEM` | Semantics | `SEM-001`–`SEM-014` | `docs/errors/SEM_ERRORS.md` |
| `RTN` | Runtime (VM) | `RTN-001`–`RTN-003` (+ planned `RTN-004`) | `docs/errors/RTN_ERRORS.md` |

Error codes are assigned in `Pirate.Cli` by `ErrorMapper`, which maps
`*ErrorKind` enum values to code strings. The compiler libraries never
see or emit codes — they only produce typed errors with enum kinds.

See [`docs/errors/ERRORS.md`](../errors/ERRORS.md) for the user-facing
catalog and [`docs/design/ERROR_CODES.md`](../design/ERROR_CODES.md)
for how the system works in code.

## Current state

**Completed:**
- ✅ `Pirate.Lexer` — C#, single-pass, source locations, diagnostics, 83 tests; knows `const`/`import`/`export`
- ✅ `Pirate.Parser` — F#, Pratt expression parser + recursive-descent statements, 59 tests; imports, exports, top-level statements, inferred-`const`
- ✅ `Pirate.Syntax` — 23 AST node types (`*Node` suffix), `CompilationError` hierarchy, semantic-model data types (`PirateType`, `Symbol` family)
- ✅ `Pirate.Semantics` — symbol table, builtin registry, two-pass analyzer (signatures → top-level code → function bodies), full static type checking incl. operator typing, 149 tests
- ✅ `Pirate.Shared.File` — file discovery and naming helpers, tests
- ✅ `Pirate.Fleet` — `.fleet` manifest model, read/write, entry-point resolution, tests
- ✅ `Pirate.Cli` — 5 commands on Spectre.Console with DI composition (`TypeRegistrar`), content-hash build cache gated on a clean frontend, `CompilationPipeline` (lexer → parser → semantics) wired into build/run/shell, error rendering incl. SEM codes, run-entry rule (entry module needs top-level statements), tests
- ✅ `Pirate.Shared.Logging` — `ILogger`, `NullLogger`, `ConsoleLogger`, tests
- ✅ `.agents/` — model/provider-agnostic agent instructions

**In progress:**
- 🔧 Module linking — `import module` / `import external` resolution against the fleet manifest, export visibility, entry-only top-level code (SEM-013 today)

**Not yet started:**
- ⬜ `Pirate.Compiler` — bytecode emission (interleaved with VM)
- ⬜ `Pirate.VM` — stack machine, call frames, `PirateValue` (interleaved with compiler)
- ⬜ `Pirate.StandardLibrary` — native function implementations
- ⬜ `Pirate.Spec.Test` Reqnroll scaffolding — e2e scenarios assert real stdout once execution lands
