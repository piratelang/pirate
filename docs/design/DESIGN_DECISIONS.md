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

## [v2-019] `docs/STYLE.md` adopted as the v2-only style standard

**When**: 2026-09-28
**What**: A single style guide, `docs/style.md`, now governs all code under
`src-v2/`: no shortened identifiers and full qualification of shadowing
names; per-project `IServiceCollection` extensions with everything
injectable into the CLI (interfaces for pipeline stages, abstractions-only
package reference in libraries); shared features extracted into
`Pirate.Shared.*` projects when two or more projects need them; F# retained
but documented purely by its v2-native idioms (modules, explicit state
records, pattern matching); comments must explain v2 behavior self-contained
— v1 comparisons belong in `docs/GRAMMAR_CHANGES.md`, not in code; testing
may use helper libraries (FakeItEasy/AutoFixture/FluentAssertions) with
shared fixtures extracted on duplication. The guide deliberately contains no
v1 style section — v1 is legacy and not a model.
**Why**: v2's existing code had drifted into per-stage conventions that
contradicted each other on naming brevity, static-vs-injected composition,
and comment framing. Writing the standard down — and listing the three
existing deviations (rename `LeftParen`/`RightParen`-style names; convert
static `Lexer.Tokenize`/`Parser.Parse` to injected services; strip v1
cross-references from v2 doc comments) as a "Migration appendix" — fixes the
rules now without bundling a pipeline-wide refactor into a docs change. The
appendix items are follow-up work each landing as its own change, so old
code stays a known, tracked exception instead of silent precedent.
---

## [v2-020] Top-level statements replace `func main()`

**When**: 2026-09-28
**What**: A module is `{ import | extern | export | func | statement }`.
The entry module that the fleet resolves for `pirate run` executes its
top-level statements; the historic "valid programs must define a zero-param
`func main() : void`" requirement is removed from GRAMMAR.md §3.1. Missing
entry code is a *run-time* concern (`EntryPoint.HasRunnableBody` in
`Pirate.Cli`), to be formalized as RTN-004 when the VM lands.
**Why**: `docs/examples/` was adopted as the syntax spec (user decision),
and every example program is top-level code with no `main`. Enforcement at
run rather than check time keeps helper modules valid to build (a helper is
declarations only; even today, a module without top-level code builds
clean). Rejecting top-level statements inside *imported* modules waits for
the linker (see v2-021).

---

## [v2-021] `import` / `export`: surface now, linking later

**When**: 2026-09-28
**What**: `import standard <NS>;` binds every builtin of `Standard.<NS>`
into global scope under both its leaf name and dotted path;
`import module|external <path> [as <alias>];` and `export` are defined by
the grammar and parse, but the analyzer reports SEM-013
`ModuleImportUnsupported` for module/external imports, and `export` marks
are recorded without effect. `extern` stays as the single-name low-level
form; mixing both for one function is a duplicate declaration (SEM-010).
**Why**: Committing the syntax now locks the spec that
`docs/examples/4 - Multi Module` and `5 - External Module` describe, gives
the parser/lexer work a complete surface to test against, and lets the
module-linking milestone land as pure semantics+CLI (fleet manifest →
module graph → export resolution) without any grammar churn.

---

## [v2-022] Semantic-model data types live in `Pirate.Syntax`

**When**: 2026-09-28
**What**: `PirateType` and the `Symbol` family (`VariableSymbol`,
`FunctionSymbol`, `BuiltinSymbol`, `SymbolScope`) are plain data in
`Pirate.Syntax`; the resolution machinery (`SymbolTable`,
`SemanticAnalyzer`) stays in `Pirate.Semantics` (v2-012 unchanged). The
"checked AST" is the *same* tree: the analyzer annotates nodes through
nullable mutable properties outside the record's positional parameters
(`ExpressionNode.InferredType`, `ResolvedSymbol` on declarations/parameters,
`LoopVariable` on the for nodes, `ResolvedCallee` on calls).
**Why**: A side-table keyed on nodes is unsafe because records have
structural equality (two identical subexpressions collide); a parallel
"checked" tree duplicates 23 node types. Positional parameters can't grow
per-pass fields (every construction site churns), but init/mutable
properties added to record bodies annotate in place with zero copies, keep
`Pirate.Syntax` dependency-free, and let `Pirate.Compiler` (later) consume
symbols without referencing `Pirate.Semantics`.

---

## [v2-023] Blocks do not open scopes

**When**: 2026-09-28
**What**: The scope tiers are exactly Global (module: imports, externs,
functions, top-level declarations) / Local (one per function: params,
locals, loop variables) / Builtin / Free (reserved, closures). An
`if`/`while`/`for` body introduces no scope; loop variables stay visible
after the loop, and re-declaring inside nested blocks is SEM-010.
**Why**: Matches the architecture doc's table and Thorsten Ball's compiler
(function scope + slot indexes). Block scoping would complicate slot
numbering for no v2 requirement; recorded as an open item in GRAMMAR.md §4.

---

## [v2-024] Operator typing follows the Ball defaults; anything beyond is an issue

**When**: 2026-09-28
**What**: `+` on int/int and string/string (concat), `- * /` on same-type
numerics, `< <= > >=` on same-type numerics, `== !=` on any same-type
non-array, `&& || !` on bool, `% ^` int-only, `T[]` indexed by int. No
implicit widening (already grammar §2). The wider domains — float `%`/`^`,
char/string relational ordering, string indexing, array equality — were
deliberately NOT invented: they are rejected by the analyzer and filed as
follow-up issues, added to GRAMMAR.md §4.
**Why**: Ball's language is the agreed reference ("check Thorsten Ball for
default behaviour"), it has no answer for these cases (one numeric type,
no char ordering), and inventing type rules that the spec hasn't chosen is
how v1's docs and implementation drifted apart.

---

## [v2-025] `const` is explicit immutability, everything else is mutable

**When**: 2026-09-28
**What**: `const` (with optional type or `var`: `const int X = 5;`,
`const name = "x";`) marks a variable immutable; any later assignment —
including element writes (`const` list `a[i] = v` blocked too) — is
SEM-004. Parameters, loop variables, and unmarked declarations are mutable
subject to exact-type reassignment (SEM-003 on mismatch).
**Why**: User decision ("const is explicit immutable, everything else is
mutable with type safe"). Blocking element writes is the conservative
reading of "immutable"; deep-vs-shallow const remains open if the spec ever
wants it. SEM-004's old catalog text ("use var to declare") predates const
existing and was rewritten to match.

---

## [v2-026] New stages are DI services; existing static seams wait for STYLE item (b)

**When**: 2026-09-28
**What**: `Pirate.Semantics` exposes `ISemanticAnalyzer` +
`AddPirateSemantics()` (singleton; the analyzer resets per call),
`Pirate.Cli` exposes `ICompilationPipeline` (internal implementation,
constructor-injected into commands) and composes the app through
Spectre's `TypeRegistrar`/`TypeAdapter` over `IServiceCollection`. Lexer
and Parser remain static entry points called inside the pipeline — their
conversion (`ILexer`, `IParser`) is STYLE.md migration item (b), a
dedicated follow-up change, and new code must not treat the static calls
as precedent.
**Why**: docs/STYLE.md is binding for new code the moment it lands;
converting the two existing static seams touches the F# parser seam, every
command, and several test projects, and is listed in the appendix as its
own change precisely so it doesn't balloon unrelated work.

---

## [v2-027] Error-code bookkeeping: new SEM codes, SYN-040 retired

**When**: 2026-09-28
**What**: `SemanticsErrorKind` grew SEM-011 `UnknownExtern`, SEM-012
`UnknownImport`, SEM-013 `ModuleImportUnsupported`, SEM-014
`ReturnAtTopLevel`; `SyntaxErrorKind` grew SYN-028
`MissingIdentifierAfterConst` and SYN-044..048 (import/export shapes);
SYN-040 `ExpectedTopLevelDeclaration` was **deleted** — top-level
statements make "expected extern or func" unreachable, and v2 is
pre-release, so the catalog carries no dead kinds (the number stays
unused).
**Why**: Unreachable kinds mislead contributors about what the parser can
emit (per [v2-006]'s enum↔code 1:1 promise). Deleting rather than
reserving is safe only because no released compiler has emitted SYN-040.
---

## [v2-028] Build cache is stamped with the front-end version

**When**: 2026-09-28
**What**: `.pirate/cache.json` gained a `pipelineVersion` field
(`BuildCache.PipelineVersion`, starting at 2 since v1-era caches had no
field). A cache whose stamp is missing or differs from the running
front-end is treated as empty on load; bump the constant whenever passing
`build` would mean something different than before — new pipeline stages,
grammar changes that newly reject code, cache format changes.
**Why**: Content-hash matching alone answers "has this file changed?", not
"was it ever checked by what now counts as a check" — caches created while
`build` was parse-only would have marked semantically broken modules
"(up to date)" forever. Discarding wholesale costs one rebuild and keeps
"(up to date)" honest; it also pre-solves the identical problem for the
bytecode artifacts the compiler will cache.

---

## [v2-029] Standard imports bind atomically and take no alias

**When**: 2026-09-28
**What**: `import standard <NS>;` precomputes every binding it would
introduce (each builtin under its dotted path *and* its leaf name), checks
all names against global scope, and then binds **all of them or none** — a
collision (e.g. a user `PrintLine` function) rejects the whole group with
one SEM-010, leaving the namespace unbound. An `as` alias on a `standard`
import is a syntax error (SYN-049); aliases belong to `module`/`external`
imports.
**Why**: Binding entry-by-entry left half-imported namespaces after an
error — `Print` callable, `PrintLine` not — an inconsistent state the
linker would inherit. All-or-nothing makes a failed import indistinguishable
from an absent one, which is also what the linker needs for module imports.
`standard` binds the group's own names by design, so there is nothing for
an alias to name; accepting and ignoring it would let `import standard
Terminal as T;` silently do nothing useful.

---

## [v2-030] Flat files adopted: a file is a type, `export` is removed

**When**: 2026-09-30
**What**: `docs/brainstorm/FLAT.md` and `FLAT_PLAN.md`'s decisions are
folded into `GRAMMAR.md` §4 as the canonical (if not-yet-implemented) spec,
tracked by epic #219 and sub-issues #220–228
(`docs/brainstorm/FLAT_PLAN.md`'s "Status" checklist). Classes are a file
kind (`.cpirate`/`.cpir`), not a `class Foo { }` wrapper — the filename is
the type name, every top-level member belongs to it. `export` is removed
everywhere (modules and classes alike); `private` is the one visibility
modifier, public-by-default otherwise. This work happens on branch
`feature/flat-files`, sequenced as eight phases (0 spec → 1 front-end prep →
2 lexer/parser → 3 project model → 4 semantics → 5a compiler/VM/stdlib → 5b
objects → 6 docs), landing back to back as one effort.
**Why**: OOP was already reserved (`class`/`new`) but undefined; flat files
reuse v2's existing "declarations at top level" model instead of adding a
second, wrapper-based declaration style, and remove `export` in favor of one
visibility rule shared by modules and classes. The module-linking milestone
(v2-021, "In progress" in `v2-architecture.md`) is sequenced as Phase 3 of
this plan and **must not** be built against the old `export` model first —
resolution is written once, flat-first, so a pre-flat-files module-linker
implementation was stashed on `dev` rather than carried onto this branch
(see `git stash list` on `dev`) to avoid exactly that.

---

## [v2-031] Field access has exactly three levels: public, `readonly`, `private`

**When**: 2026-09-30
**What**: `field int x` (read/write from anywhere), `readonly field int x`
(read anywhere, assigned only inside the class's own constructors/methods),
`private field int x` (neither). `readonly` applies to fields only — on a
method, constructor, or `const` it's a compile error, since there's nothing
to write there to begin with. Independent read/write restriction,
`protected` (needs `extends`), and accessor blocks (`prop`) are backlog
(GRAMMAR.md §5, "After the first slice").
**Why**: User decision, favoring the smallest access model that covers the
worked examples (`Counter`, `shop`) over speculative generality. `readonly`
doesn't freeze the *value* (a `readonly` field holding an object still lets
callers invoke that object's public methods) — only who can reassign the
field itself from outside the class.

---

## [v2-032] Every type gets a uniform nullable form, `T?`

**When**: 2026-09-30
**What**: `T?` is defined for every type — scalars, classes, and arrays
(`int?`, `Item?`, `Item[]?`, `Item?[]`) — with one rule instead of a
per-category one. `null`, `== null`/`!= null`, and flow narrowing (locals
and parameters only, not fields) are in the first slice; `?.`, `??`, `x!`
are backlog. A nullable value can't be used as its base type until narrowed
(GRAMMAR.md §2).
**Why**: One rule for scalars, classes, and arrays is simpler to specify and
implement than a scalar/reference split, and misuse being a *compile-time*
error (never a null-reference crash) is the point — there's deliberately no
run-time null failure to report once this lands. The VM's representation of
a null scalar is the highest-risk unresolved part of this decision: it's
**not** chosen here, see v2-033.

---

## [v2-033] Null representation in `PirateValue` deferred to Phase 5b

**When**: 2026-09-30
**What**: v2-032 specifies nullable scalars (`int?`) at the language level,
but how `PirateValue` (the VM's tagged-union value struct, currently `{
ValueKind Kind; double Number; object? Ref; }` per `v2-architecture.md`)
represents a null `int` is **not decided yet**. Phase 5a (compiler/VM
core, arithmetic through builtins) must not lock a `PirateValue` layout
that can't hold a null scalar; the representation is chosen at the start of
Phase 5b, before `OpNew`/`OpGetField`/`OpNull` are written.
**Why**: Getting this wrong after Phase 5a ships would mean revisiting
every opcode that touches `PirateValue`. Deciding it once, right before the
opcodes that need it, costs one design pass instead of a rewrite; flagged
explicitly in `FLAT_PLAN.md`'s risks section for the same reason.

---

## [v2-034] `self`, `constructor`, `new`: three separate keywords, one job each

**When**: 2026-09-30
**What**: `constructor` defines a constructor (reads as `func`'s
counterpart); `new Counter(10)` is the only way to create an instance; and
`self` is the current instance inside a method/constructor body *or* a
constructor delegate (`constructor() : self(16) { }`) — never a type. The
type is always spelled by the file's own name, inside the file and out
(`func add(Counter other) : Counter`). A member can't share the file's name,
since that name is in scope as a type throughout the body.
**Why**: Splitting "define," "instantiate," and "refer to the instance"
across three keywords means a definition and a call never share one
(`new` only ever means "create"), and `self` never needs a position-based
double meaning ("is this `self` the type or the instance?"). The cost is
that renaming a file touches its own body and every importer — treated as a
refactor tooling should handle, not a reason to let `self` stand in for the
type.

---

## [v2-035] Phase 2 lands: class-file grammar, `export` removed from code

**When**: 2026-09-30
**What**: The lexer/parser side of flat files (Phase 2 of
`docs/brainstorm/FLAT_PLAN.md`, #222) is implemented on
`feature/flat-files`: `field`, `constructor`, `readonly`, `self`, `null`
tokens; `super`/`extends`/`implements`/`abstract`/`override`/`static`
lexed but rejected everywhere they'd appear with one
`ReservedKeywordNotSupportedYet` error per occurrence; `new` promoted to
an expression keyword; `T?`/class-name types; and a class-file top-level
grammar (fields, constants, constructors with an optional `: self(...)`
delegate, methods — no loose statements). `export` is gone from the
lexer and parser; `FunctionDeclarationNode`/`VariableDeclarationNode`'s
`IsExported` is renamed `IsPrivate`, set by the new `private` modifier
(GRAMMAR.md §3.3).
**Why**: v2-030 already made this the canonical spec; this is that spec
implemented. `export`'s removal and `private`'s addition are one change
because they share the same grammar slot (a module-element's leading
modifier) — landing `private` without first deleting `export` would mean
two competing visibility keywords for one commit's length, which is worse
than the larger diff. None of this is reachable from `pirate build`/`run`
yet: `Pirate.Shared.File` still only discovers `.pirate`, so a `.cpirate`
class file has no way to reach the parser outside a test — that's Phase 3.

