# Design Decisions

Record of key architectural decisions made during v2 development.
Update this file whenever a design choice is made that future-you or another
agent needs to know about. Format: **What, Why, When, Who** (like ADR-lite).

---

## [v2-001] v2 is a ground-up rewrite, not a port of v1

**When**: 2026-09-22
**What**: v2 (`src-v2/`) shares no code or projects with v1 (`src/`). Both
solutions must build independently.
**Why**: v1's architecture had fundamental issues (tree-walking interpreter,
no static typing, no operator precedence, whitespace-stripping lexer). A port
would carry v1's structural problems into v2. A rewrite lets us get the
pipeline right: single-pass lexer → Pratt parser → semantics → bytecode
compiler → stack VM.

---

## [v2-002] Parser written in F#, Lexer in C#

**When**: 2026-09-22
**What**: v2 lexer stays C# (already complete, 372 lines, 34+ tests).
v2 parser is F# (replacing the C# stub).
**Why**: Pratt parsing maps naturally to F# pattern matching and discriminated
unions. The lexer is already done in C# and doesn't benefit as much from F# —
its character dispatch is already a clean `switch`. The F# parser consumes
C# `Token` structs with zero interop overhead.

---

## [v2-003] AST nodes use v1 `*Node` naming suffix

**When**: 2026-09-22
**What**: AST record types use names like `VariableDeclarationNode`,
`IfStatementNode`, `FunctionCallNode` — matching v1's naming convention.
**Why**: Familiarity for contributors who know v1. Shorter names like `VarDecl`
or `IfStmt` are cleaner but break the mental model and make it harder to trace
v1→v2 correspondence.

---

## [v2-004] No `INode` interface hierarchy in v2

**When**: 2026-09-22
**What**: AST nodes are plain C# records with no shared interface (no `INode`,
no `IExpressionNode`, etc.).
**Why**: v1's interface hierarchy was used for `IsValid()` checks which were
tautological (PAR-007, INT-006). v2 uses a separate semantics pass for real
validation, so interfaces add nothing. Records give structural equality,
pattern matching, and `with` expressions for free.

---

## [v2-005] Collect-don't-throw pipeline

**When**: 2026-09-22
**What**: Every pipeline stage (lexer, parser, semantics) collects errors into
a list instead of throwing on the first one. `LexResult` and `ParseResult`
always produce an output, even when errors occur.
**Why**: Real compilers report all errors in one pass. v1's `ParserException`
stopped at the first error, forcing fix-and-rerun cycles.

---

## [v2-006] Error codes assigned in CLI only

**When**: 2026-09-22
**What**: `CompilationError` subclasses (`LexError`, `SyntaxError`,
`SemanticsError`) carry a fine-grained `*ErrorKind` enum. Error codes
(`SYN-005`, `LEX-002`) are assigned by `ErrorMapper` in `Pirate.Cli`,
not by the parser/lexer themselves.
**Why**: Separation of concerns. The compiler library knows *what kind* of
error occurred; the CLI knows *how to present it* to the user. Adding a new
error means: add enum value, add switch arm in `ErrorMapper`, add docs entry.
No string matching, no coupling.

---

## [v2-007] `EndLocation` nullable on `CompilationError`

**When**: 2026-09-22
**What**: `CompilationError` has `StartLocation` (always set) and
`EndLocation?` (nullable). `null` for point errors, populated for range
errors (unterminated strings, unexpected tokens spanning multiple characters).
**Why**: Cleaner semantics — `null` means "this is a point, not a range."
The renderer handles both: single `^` for point, `^^^^^^^` for range.

---

## [v2-008] Build cache uses content hashing

**When**: 2026-09-22
**What**: `.pirate/cache.json` stores `SHA256` hashes of file contents, not
`LastWriteTime`. Only files whose hash changed are rebuilt.
**Why**: Catches manual reverts (editing a file back to a previous version
would fool `LastWriteTime`). Slower than timestamp comparison but correct.
The hash cost is negligible compared to lexing/parsing.

---

## [v2-009] Error docs split external/internal

**When**: 2026-09-22
**What**: `docs/external/ERRORS.md` + per-type catalogs (LEX, SYN, SEM, RTN)
for users. `docs/internal/ERROR_CODES.md` for contributors (how the system
works in code, how to add new errors).
**Why**: Users need to know what an error means and how to fix it.
Contributors need to know which files to touch and how the enum→code mapping
works. These are different audiences with different needs.

---

## [v2-010] Shorter `*ErrorKind` enum names

**When**: 2026-09-22
**What**: `SyntaxErrorKind.MissingSemicolonAfterDeclaration`, not
`SyntaxErrorKind.SemanticsExpectedSemicolonAfterVarDeclarationStatement`.
Enum names are concise; context (the type name) tells you the category.
**Why**: Readability in parser code. `SyntaxError(MissingSemicolonAfterDeclaration, ...)`
reads clearly. Overly qualified names add noise without adding information.

---

## [v2-011] Bytecode and VM grow together (Thorsten Ball pattern)

**When**: 2026-09-22
**What**: Compiler and VM are built simultaneously, starting with the smallest
working unit (1-2 opcodes, push/execute `1+1`) and growing in lockstep.
Not sequential (full compiler → then full VM).
**Why**: From Thorsten Ball's "Writing a Compiler in Go" — avoids speculating
about bytecode before you know how the VM will execute it. Each phase adds
opcodes to both sides at once, providing immediate feedback.

---

## [v2-012] Symbol table in Semantics, not Compiler

**When**: 2026-09-22
**What**: `SymbolTable` lives in `Pirate.Semantics`, not `Pirate.Compiler`.
The semantics pass resolves all names, annotates nodes with resolved symbols
and types. The compiler consumes resolved symbols to emit index-based opcodes.
**Why**: Matches the v2 architecture doc's pipeline design. Name resolution
and type checking are semantic concerns; the compiler just translates an
already-checked AST into bytecode.

---

## [v2-013] Error codes use 3-letter prefix + sequential number

**When**: 2026-09-22
**What**: `LEX-001` (lexer), `SYN-005` (parser), `SEM-003` (semantics),
`RTN-001` (runtime). Gap numbering within each prefix allows inserting new
errors without renumbering.
**Why**: Consistent with the existing issue tracker (`LEX-007`, `PAR-007`,
etc.) but extended to user-facing codes. Gaps (001-014, 020-027, etc.) make
it easy to add errors later.

---

## [v2-014] Build cache in hidden `.pirate/` folder

**When**: 2026-09-22
**What**: Content-hash cache stored at `.pirate/cache.json`, not in the
project root or a visible dotfile.
**Why**: Keeps the project root clean. The `.pirate` folder can later hold
other build artifacts (compiled bytecode, dependency caches, etc.) without
cluttering the user's directory listing.

---

## [v2-015] Single shared mutable parse cursor (`State`)

**When**: 2026-09-22
**What**: The Pratt expression parser and the statement/top-level parser share
one `internal State` record (defined in `ParserResult.fs`, the first-compiled
file): `{ Tokens; mutable Pos; Errors }`. Both modules take and mutate this one
cursor in place — no per-call sub-state copies (`{ state with ... }`) and no
copying `Pos` back manually after a call.
**Why**: The parser must advance one cursor through nested expression +
statement calls. F# records are immutable except for explicitly `mutable`
fields, so a `copy-and-update` of a record with a `mutable Pos` field creates a
*new* record and the original's `Pos` never moves — expressions parse correctly
but the statement parser then re-reads the same tokens forever. Threading a
single reference-shared cursor is the idiomatic fix (mirrors the mutable
index/token-list in Thorsten Ball's Go parser). The `advance` helper also clamps
so `Pos` can never pass the trailing `Eof`, preventing peek-index out-of-range
after an error path consumes the end of the stream.

---

## [v2-016] Assignment statements detected in the statement parser, not Pratt

**When**: 2026-09-22
**What**: `x = 5;` and `list[i] = v;` (GRAMMAR.md §3.4 `assignment-statement`)
are recognized by `parseExprStmt`: it parses a leading expression via Pratt,
then if the next token is `=` parses the value and reinterprets the LHS
(`QualifiedNameNode` → plain assignment; `IndexExpressionNode` → element
assignment). `=` is deliberately *not* a binary operator in the precedence
table.
**Why**: An assignment LHS is not a value expression, so it can't flow through
the expression-returned-by-Pratt model as an operator. Handling it at statement
level keeps `=` out of the precedence table (where `==` is `EqualEqual`) and
lets the parser distinguish declaration (`var x =`, `int x =`) from reassignment
(`x =`) by the leading keyword. An invalid target degrades to
`ExpressionStatementNode` with a recorded `ExpectedExpression` error rather than
throwing, per the collect-don't-throw rule.

---

## [v2-017] `ParseResult.Errors` holds only parser errors, not lexer errors

**When**: 2026-09-22
**What**: `Parser.Parse` seeds a *fresh* error list — it does **not** copy the
incoming `LexResult.Errors` into `ParseResult.Errors`. The CLI explicitly
concatenates `lexResult.Errors` + `parseResult.Errors` for display. Each stage
reports only its own errors.
**Why**: Keeps stage boundaries clean (the lexer owns `LexError`s, the parser
owns `SyntaxError`s) and matches the architecture doc's pipeline diagram, which
shows `LexError[]` and `SyntaxError[]` as separate side-outputs. Seeding the
lexer errors into the parser's result double-counted them at every CLI call
site, printing each `LexError` twice. Concatenating in the CLI makes the "all
errors in one pass" behaviour explicit rather than hidden in the parser.

---

## [v2-018] Cache is content-hash keyed on the real project root

**When**: 2026-09-22
**What**: `BuildCache` stores the project directory it was loaded from and keys
relative paths off it (not off `Path.GetDirectoryName(_cachePath)`, which is the
`.pirate` subfolder). Saves are atomic (temp file + rename) and `Load` treats a
corrupt/unreadable `cache.json` as empty instead of throwing.
**Why**: The `.pirate` folder is an implementation detail; relative module keys
must be stable regardless of where the cache lives. A cache is a performance
optimization — losing it (fall back to a rebuild) must never crash the CLI, and
a half-written cache from a crash must not poison the next run.

---
