# Pirate — instructions for agents working in this repo

Before making any change in this repo, read the relevant docs in `docs/`:

- [`docs/GRAMMAR.md`](../docs/GRAMMAR.md) — canonical v2 language grammar (lexical rules, types, full statement/expression grammar). Any change to v2 syntax or semantics must be reflected here in the same change.
- [`docs/GRAMMAR_CHANGES.md`](../docs/GRAMMAR_CHANGES.md) — why v2 differs from v1 (dropped keywords, static typing, arrays, etc.). Read this before assuming v1 behavior carries over.
- [`docs/TESTING.md`](../docs/TESTING.md) — testing strategy: xUnit per project, Gherkin/Reqnroll end-to-end. Any grammar or pipeline change needs unit tests in the matching `*.Test` project and, for syntax/semantics changes, an e2e scenario in `Pirate.Spec.Test`.

Root `GRAMMAR.md` and `SYNTAX.md` (outside `docs/`) describe **v1 only** and are historical — do not use them as a source of truth for `src-v2/` work, and do not edit them as part of v2 work.

## Repo architecture, in general

There are two complete, independent implementations of the Pirate language in this repo, in separate solutions, sharing nothing at the project level:

- **v1** — `src/PirateLang.sln`. The shipped implementation: F# lexer (`Pirate.Lexer.F`) → C# recursive-descent parser (`Pirate.Parser`) producing an AST of `INode`-derived classes → a tree-walking interpreter (`Pirate.Interpreter`) that evaluates that AST directly, dynamically typed. `Shell` is the CLI (hand-rolled command dispatch). `Pirate.Common*` holds cross-cutting exception/logging/file-handling libraries. Still the version users run; do not break it while v2 is in progress.
- **v2** — `src-v2/PirateLang.slnx`. A ground-up rewrite, not a port. See below.

Do not add cross-references between `src/` and `src-v2/` projects — they are meant to build, run, and be deleted independently. `src/` goes away entirely once v2 reaches parity (see the migration plan discussed with the user; there is no migration doc checked in yet — ask before assuming a cutover step is already decided).

## v2 architecture, specifically

v2's goals: a CLI built on Spectre.Console, a faster single-pass lexer and a cleaner parser, and a genuinely **compiled** pipeline (bytecode + VM) instead of v1's tree-walking interpreter, with **static typing** checked before a program ever runs.

Project graph (`src-v2/PirateLang.slnx`):

```
Pirate.Syntax  ←  Pirate.Lexer  ←  Pirate.Parser  ←┐
       ↑                                            ├─ Pirate.Cli (Spectre.Console.Cli)
       └────────  Pirate.Semantics  ─────────────────┤
                                                      │
Pirate.VM  ←  Pirate.Compiler ───────────────────────┤
     ↑                                                │
     └── Pirate.StandardLibrary ──────────────────────┘
```

- **Pirate.Syntax** — plain AST record types shared by everything downstream of parsing. No logic, no tests of its own (see `docs/TESTING.md`).
- **Pirate.Lexer** — source text → tokens. Single pass over the source, no upfront mutation of the input (v1's lexer stripped newlines before lexing, which destroyed line info — v2 must not repeat that), real line/column tracked per token for diagnostics.
- **Pirate.Parser** — tokens → `Pirate.Syntax` AST, per `docs/GRAMMAR.md` §3. Collects diagnostics and continues past errors rather than throwing on the first one.
- **Pirate.Semantics** — name resolution + the static type-checking rules in `docs/GRAMMAR.md` §2. This is what turns a type error into a compile-time diagnostic instead of a runtime crash.
- **Pirate.Compiler** — checked AST → bytecode (opcodes + constant pool), defined in `Pirate.VM`.
- **Pirate.VM** — the bytecode/opcode definitions and the stack-machine interpreter loop that executes them. Values are a tagged-union struct, not a class hierarchy — scalars (int/float/bool/char) must not heap-allocate.
- **Pirate.StandardLibrary** — native functions (`Standard.Terminal.Print`, `Standard.String.*`, …) registered into the VM's native-call table, resolved by an `extern` declaration.
- **Pirate.Cli** — the `Spectre.Console.Cli` entry point wiring the above into `run`/`build`/etc. commands.
- **`*.Test`** projects — xUnit, one per library project (see `docs/TESTING.md` for what each covers).
- **Pirate.Spec.Test** — Gherkin/Reqnroll end-to-end scenarios running real `.pirate` scripts through the whole v2 pipeline and asserting on actual output. This is the parity gate against v1 before v1 is deleted.

When extending v2: update the grammar doc first if syntax/semantics change, then implement bottom-up (Syntax → Lexer → Parser → Semantics → Compiler/VM → StandardLibrary → Cli), adding unit tests alongside each project and an e2e scenario for any user-visible behavior.
