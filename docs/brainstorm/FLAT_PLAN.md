# Flat files: implementation plan (brainstorm)

Status: **plan, not implemented.** Companion to [`FLAT.md`](FLAT.md), which
describes the language. This file records the decisions made so far, the
phases to build it on v2, and what is still undecided. Nothing here is
canonical until Phase 0 folds it into `GRAMMAR.md` and
`DESIGN_DECISIONS.md`.

Tracked by [#219 — v2: Flat files — a file is a type (classes first
slice)](https://github.com/piratelang/pirate/issues/219), one sub-issue per
phase: [#220](https://github.com/piratelang/pirate/issues/220) Phase 0,
[#221](https://github.com/piratelang/pirate/issues/221) Phase 1,
[#222](https://github.com/piratelang/pirate/issues/222) Phase 2,
[#223](https://github.com/piratelang/pirate/issues/223) Phase 3,
[#224](https://github.com/piratelang/pirate/issues/224) Phase 4,
[#227](https://github.com/piratelang/pirate/issues/227) Phase 5a,
[#225](https://github.com/piratelang/pirate/issues/225) Phase 5b,
[#226](https://github.com/piratelang/pirate/issues/226) Phase 6.
Superseded issues [#209](https://github.com/piratelang/pirate/issues/209)
(module linking) and [#217](https://github.com/piratelang/pirate/issues/217)
(`ILexer`/`IParser` DI seams) are closed; their scope lives in Phase 3 and
Phase 1 respectively.

**The phases are structure, not milestones.** They run back to back as one
effort, so an intermediate phase doesn't have to leave a shippable product. Each
one still lands green (`dotnet test src-v2/PirateLang.slnx`) with its tests and
docs.

## Decisions taken

| Topic | Decision |
|---|---|
| Kinds | Class `.cpirate` or `.cpir`, interface `.ipirate` or `.ipir` (later), module or entry file `.pirate` or `.pir`. The old `.class.pirate` spelling is dropped. The type name is the filename without its final extension and must be a single identifier — `foo.bar.cpirate` is an error. Two files that resolve to the same type name in one folder are a duplicate-name error, whatever the extensions. |
| Fields and constructors | `field` is required on every field. Constructors use `constructor`. `new` only creates an instance. |
| Field values | Every field gets a value, nullable or not. Either an initializer at the declaration (`= null` for a nullable field), or every constructor assigns it (definite assignment). No implicit defaults. |
| `self` | Instance and constructor delegate only, never a type. The class is spelled by its file name inside the file and out. |
| Visibility | Public by default, `private` opts out. `export` is removed everywhere. |
| Field access | Exactly three levels in the first slice: public (read and write), `readonly` (read anywhere, assigned only by the class's own constructors and methods), `private` (neither). `readonly` is for fields only. Independent read/write control, `protected` and accessor blocks are backlog. |
| Overloads | Methods overload like constructors: by arity, then parameter types, ambiguity is an error. |
| Class constants | `const int Limit = 1;` without `field`, as elsewhere. |
| Naming | Lowercase folders and namespaces, PascalCase types: convention in `STYLE.md`, not enforced. Names differing only by case still collide. |
| Root namespace | The `.fleet` file's base name. `pirate init` prompts for the project name and uses it for both the file name and the namespace. `-n\|--name` supplies it without the prompt. |
| Entry point | `main.pirate` (or `main.pir`) at the project root by default, a module file. The `entryPoint` field in the fleet file overrides it with a path relative to the root, without the extension. |
| Same-folder declarations | Classes and modules in the same folder are visible without an import. Everything else needs `import module <full name>;`. |
| Null | Every type is nullable with `T?`, including scalars. `== null` and `!= null` work on every type, including arrays. `==` between two class values compares references. Array equality (#214) stays out of scope. |
| Static | No `static` in the first slice. Shared functions and constants live in modules. |
| Module state | A non-entry `.pirate` module holds functions and `const` values only. Mutable module-level variables are an error. |
| Unsupported words | `extends`, `implements`, `abstract`, `override` and `static` are reserved. The parser reports one dedicated "not supported yet" error for each. |
| REPL | The `pirate shell` command is removed. |
| First slice | Classes only: fields, constructors, methods, `self`, namespaces, imports, nullable types. No `extends`, `implements` or interfaces. |

**Sequencing:** adopting this plan folds the in-progress module-linking
milestone (`v2-architecture.md`, "In progress") into it — Phase 3 is that
milestone. The linker must not first be built under the old `export` model
that `FLAT.md` removes: resolution is written once, flat-first.

## Assumptions to confirm

These are my defaults where the decisions above don't say.

1. **Nullable operators in the first slice:** `null`, `== null`, `!= null` and
   flow narrowing inside `if x != null { }`. `?.`, `??` and `x!` come later.
2. **A nullable value can't be used as its base type** until it is narrowed.
   `int? a; a + 1` is a compile error. Narrowing applies to plain local
   variables and parameters, not to fields.
3. **A class `const`** is instance-independent and folded at compile time.
   With no `static`, this is the only shared-value form inside a class.
4. **Field initializers run in declaration order,** before the constructor body.
   An initializer may not read a later field or call a method on `self`.
5. **Declared-but-unassigned locals** stay illegal (`int x;` needs an
   initializer), so definite assignment only applies to fields.
6. **Arrays of classes** (`Item[]`, `Item?[]`, `Item[]?`) are allowed. Nested
   arrays stay out of scope.

## Before starting

- Confirm `dotnet test src-v2/PirateLang.slnx` passes on `dev`, so Phase 1's
  rewritten goldens can be told apart from existing failures.
- Commit `docs/brainstorm/` and work on a feature branch off `dev`.

## Phases

Each phase lands as its own change, with tests, docs and a
`DESIGN_DECISIONS.md` entry, following the repo's existing pattern.

### Phase 0: write the spec

- Fold `FLAT.md` and this file's decisions into `GRAMMAR.md` (new sections),
  `GRAMMAR_CHANGES.md`, `DESIGN_DECISIONS.md` (v2-030 onwards), `FLEET.md` and
  `docs/errors/`.
- Remove `export` from the grammar and examples 4 and 5.
- Remove the `shell` command from `CLI.md`.
- Update `docs/architecture/v2-architecture.md`: the module-linking "In
  progress" item becomes this plan's Phase 3, and `GRAMMAR.md` §4's
  `class`/`new` open item closes.
- Add example `docs/examples/6 - Classes` (the Counter and shop projects) as the
  syntax spec, per the decision that examples are the spec.
- Settle the two open grammar questions before Phase 2 freezes them: whether
  `const` in a class file needs `field`, and how a bodiless interface method is
  written once interfaces arrive.
- Note in `DESIGN_DECISIONS.md` that the null representation in `PirateValue`
  is decided in Phase 5b, so Phase 5a doesn't lock a layout that can't hold a
  null scalar.
- Add a `CHANGELOG.md` entry for the breaking changes: `export` removed, new
  extensions, `pirate init` prompt, `shell` removed, `PipelineVersion` bump.
- Confirm nothing in `src/` (v1) or its docs is affected, and state in the
  README that the flat-file changes are v2-only.

Done when: an agent could implement from the docs alone, and the new example
reads well.

### Phase 1: prepare the front end

Do this **before** the compiler consumes the AST.

- Convert `Lexer.Tokenize` and `Parser.Parse` to `ILexer` and `IParser`
  (STYLE migration item b). The parser takes the file kind as an input.
- Remove the `pirate shell` command, its tests and its docs.
- Generalize `PirateType` and `TypeNode`: a scalar or class-name base, an
  `IsNullable` flag and an array flag. Move the scalar-only rules in the
  analyzer behind helper methods rather than editing each one.
- Replace `QualifiedNameNode.Parts` chains with a member-access postfix
  (`a.b`, `a.b()`), and keep a resolver for namespace-prefix names.
- Add assignment targets through member access (`self.count = 1;`).

Tests: lexer output unchanged; analyzer behavior unchanged, but the parser
goldens for dotted calls (`Standard.Terminal.Print(x)` becomes nested
member-access nodes, not one qualified name) are rewritten, plus new parser
tests for the postfix forms. Budget for that rewrite — it is the honest cost
of this phase, not a regression.

### Phase 2: lexer and parser

- Keywords: `field`, `constructor`, `private`, `readonly`, `override`, `abstract`,
  `extends`, `implements`, `self`, `super`, `null`. Promote `new` to an
  expression keyword. `static` is reserved.
- Productions from `FLAT.md`: class file, `field`, `constructor`, `new`
  expression, `T?` types, `null` literal, `self`. A `field` initializer may be
  omitted syntactically, and semantics enforces definite assignment.
- A class file is parsed with the class-file entry rule, a module with the
  existing rule. Loose statements in a class file are a syntax error.
- Parser errors for the new shapes: missing `field`, constructor delegate
  shape, misplaced modifiers, and one dedicated "not supported yet" error for
  each reserved-but-unsupported word.

Tests: per-production parser tests, error-recovery tests, and a golden AST for
the `Counter` and `Money` examples.

### Phase 3: project model and namespaces

- `pirate init` prompts for the project name and writes `<name>.fleet`. `-n`
  supplies the name without the prompt. Refuse names that collide with reserved
  roots (`standard`).
- The entry point defaults to `main.pirate` in the root, and `entryPoint`
  overrides it with a root-relative path without the extension.
- File identity becomes path plus kind: `Shared.File` learns the six
  extensions (`.pirate`, `.pir`, `.cpirate`, `.cpir`, `.ipirate`, `.ipir`), and
  `FleetEntryPoint` resolves `entryPoint` relative to the root. Discovery
  compares the exact extension after enumerating, because on Windows a
  three-character pattern such as `*.pir` also matches `.pirate`. Reject file
  names whose type segment contains a dot (`foo.bar.cpirate`), reject two files
  in one folder that resolve to the same type name whatever their extensions,
  reject an ambiguous entry (`main.pirate` and `main.pir` together), and reject
  an `entryPoint` that names a class or interface file.
- Build the project type registry: every file is registered before any body is
  checked, keyed by `namespace.path.Name`. Detect duplicate full names and a
  type sharing its name with a sibling folder, compared case-insensitively.
- Import graph: `import module a.b.C`, same-folder visibility, `as` aliases.
- Cache hash includes each file's imports (so an edit invalidates dependents).
  Bump `PipelineVersion`.
- `dependencies` in the fleet file (path map) and the `import external`
  resolver, checked against each dependency's file name.

Tests: `Pirate.Fleet.Test`, `Pirate.Shared.File.Test`, CLI tests for `init`,
cache-invalidation tests across a three-file dependency chain.

### Phase 4: semantics

- Project-wide analyzer: register types, resolve imports, then check bodies.
  `ISemanticAnalyzer` takes a project, not one program. SEM-013 retires here,
  where cross-file resolution first exists.
- **Decide block scoping (#216) first,** since flow narrowing and definite
  assignment are block-based and the decision changes local slot numbering.
- Symbols: a Class tier, field, method and constructor symbols, `self` as a
  Local. Per-file import table alongside the Global tier.
- Constructors and methods: overload resolution by arity, then types
  (ambiguity is an error). Constructor delegation (circular delegation is an
  error); implicit `super()` is a no-op until `extends` exists.
- Member access: resolve namespace prefix, type, then member. Enforce
  `private` (no access from other files) and `readonly` (assignment only inside
  the class's own constructors and methods). `readonly` on a method,
  constructor or `const` is an error.
- Nullable types: `T` is a subtype of `T?`, narrowing on `== null` and
  `!= null` for locals and parameters, no use of a nullable as its base type.
- Equality: `== null` and `!= null` on every type including arrays, and
  reference equality between two class values. Array equality (#214) stays out.
- Definite assignment for every field, nullable or not.
- Module rules: non-entry modules hold functions and `const` only.
- New `SemanticsErrorKind` values and `ErrorMapper` codes (field named like
  its file, ambiguous overload, private access, nullable misuse, unassigned
  field, loose statement in a class, ...) plus `docs/errors/SEM_ERRORS.md`.

Tests: `Pirate.Semantics.Test` grows by an estimated 100 tests. One test class
per rule group.

### Phase 5a: compiler, VM core, standard library ([#227](https://github.com/piratelang/pirate/issues/227))

The existing growth plan (phases 1 to 8 in `v2-architecture.md`): constants,
arithmetic, booleans, jumps, globals, strings and arrays, calls, builtins. Plus
the standard library the builtins need, the Reqnroll spec scaffolding, and RTN-004
(#210). None of it needs classes, so it only depends on Phase 1.

### Phase 5b: objects ([#225](https://github.com/piratelang/pirate/issues/225))

- Decide how a null scalar is represented in `PirateValue` before writing
  opcodes, and record it in `DESIGN_DECISIONS.md`.
- `PirateValue` gains a null kind, and an object reference kind for `Ref`.
- Opcodes: `OpNew`, `OpGetField`, `OpSetField`, `OpCallMethod`, `OpNull`,
  `OpIsNull`. Methods are statically dispatched in the first slice (no
  `extends`), so there is no vtable yet.
- Field initializer and constructor bodies are compiled into one
  constructor function per overload.
- New `RTN` codes for any remaining run-time faults (there should be none from
  null, since null use is a compile-time error).

Tests: `Pirate.Compiler.Test` and `Pirate.VM.Test` per opcode, then
`Pirate.Spec.Test` end-to-end on the Counter and shop examples.

### Phase 6: examples and docs

- `docs/examples/6 - Classes` and `7 - Namespaces` (the shop project), each
  with a passing spec scenario.
- Update `README.md`, `CLI.md` and `.agents/AGENTS.md`.

## Documentation

Each phase updates the docs for the areas it touches, as part of its own
change. The epic closes only after the Phase 6 audit ([#226](https://github.com/piratelang/pirate/issues/226))
confirms that these all match the shipped code:

| Area | Files |
|---|---|
| Architecture | `docs/architecture/README.md`, `v2-architecture.md` |
| Design | `docs/design/DESIGN_DECISIONS.md`, `ERROR_CODES.md` |
| Errors | `docs/errors/*` |
| Examples | `docs/examples/*` |
| CLI commands | `docs/CLI.md`, `README.md` |
| Fleet structure | `docs/FLEET.md` |
| Grammar | `docs/GRAMMAR.md`, `GRAMMAR_CHANGES.md` |
| Style | `docs/STYLE.md` |
| Testing | `docs/TESTING.md` |
| Agent instructions | `.agents/AGENTS.md` |

The audit ends with a stale-term sweep for leftover `export`, `shell`,
`module.fleet`, `SEM-013`, `func main`, `member` and `Self`.

## After the first slice

Tracked as the backlog issue [#228](https://github.com/piratelang/pirate/issues/228).

1. `extends`, `implements`, `.ipirate`, virtual dispatch (vtables).
2. Nullable operators: `?.`, `??` and `x!`.
3. `static` members, if modules turn out not to be enough.
4. Enums and exhaustive `match`.
5. Generics (out of scope for now).

## Risks

- **Phase 1 is the critical one.** It touches the type model, the parser and
  the analyzer at once. Land it alone, green, before anything else.
- **Block scoping (#216)** changes local slot numbering, narrowing and
  definite assignment. Decide it at the start of Phase 4, before Phase 5a locks
  `LOAD_LOCAL`.
- **Uniform nullable scalars** mean `PirateValue` needs to represent a null
  `int`. That affects the VM's layout, so decide the representation in
  Phase 5b before writing opcodes, not after.
- **Import-aware cache invalidation** is easy to get subtly wrong. Test it with
  a dependency chain of three files.
- **Definite assignment** through constructor delegation (`: self(...)`) needs
  a rule: a delegating constructor counts as assigning whatever the target
  assigns.
