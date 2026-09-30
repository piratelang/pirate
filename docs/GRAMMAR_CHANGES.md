# Pirate v2 Grammar: Decisions and Deltas from v1

Companion to [`GRAMMAR.md`](GRAMMAR.md). That document is the grammar itself;
this document is the *why* behind the places v2 deliberately differs from
what v1's docs described or implemented. Keeping the rationale separate from
the spec means the spec stays a clean reference, and this file stays the
place to look when someone asks "wait, didn't v1 have X?".

- **Symbolic operators only.** `is` / `is not` / `and` / `or` (documented in
  v1's `SYNTAX.md` but never implemented) are dropped. Use `==`, `!=`, `&&`,
  `||`. One operator spelling, less lexer/parser surface.
- **`for` unifies counting and iteration.** There is no separate `foreach`
  keyword. `for (item in collection) { }` is a second form of the `for`
  statement, alongside the existing counting form
  `for var i = 0 to 10 { }`. `in` stays a keyword; `foreach` is retired.
- **`elif` is deferred, not in v2 scope.** v1's `SYNTAX.md` showed it but no
  parser/interpreter ever implemented it. `if` / `else` (with `else { if ... }`
  nesting for chains) is what v2 ships; `elif` can be added later as sugar
  for that nesting without changing anything else in the grammar.
- **`bool` is a first-class type.** v1 could produce boolean values (from
  comparisons) but had no `bool` type keyword and no `true`/`false` literals,
  so a boolean couldn't be declared, typed, or written as a literal. Static
  typing (below) needs every value to have a nameable type, so v2 adds `bool`
  and `true` / `false` literals.
- **Static typing, checked before compiling.** `<type> x = value;` and
  `var x = value;` both produce a variable with one fixed, known-at-compile-time
  type — `var`'s type is inferred from its initializer once, not re-inferred
  or allowed to change. Type errors (mismatched operands, wrong argument
  types, unknown names) are reported by the semantic pass as compile
  diagnostics, not discovered mid-execution like v1's tree-walking
  interpreter.
- **Arrays are in scope now.** `T[]` element type, array literals, and
  indexing are specified in the grammar rather than deferred, since v1's
  `SYNTAX.md` already showed the surface syntax.
- **`const` declares an immutable variable.** v1 had no immutability at all
  (`ValueTable` was a plain name→value dictionary). `const` requires either
  an explicit type or inference: `const int X = 5;` and `const name = "x";`
  are both valid; assigning to a `const` variable later — including element
  writes through an index — is a compile-time error. Unmarked declarations
  (`var x = ...`, `int x = ...`) stay mutable, with exact-type reassignment.
- **Top-level statements replace `func main()`.** v1's docs showed
  `main()`-as-entry; the shipped v1 also required it. v2 instead lets a
  module carry executable statements directly at top level (`PrintLine("ahoy");`),
  and `pirate run` executes the top-level statements of the fleet-declared
  entry module. Programs that define only functions are valid to build;
  they simply have nothing to run. The entry requirement is enforced at
  run time, not as a semantic error — helper modules are declarations, and
  requiring "no top-level code in imported modules" belongs to the module
  linker.
- **`import` / `export` are the module-system surface.**
  `import standard Terminal;` binds a builtin group by leaf name (v1 forced
  the dotted path through `extern`; `extern` remains as the low-level
  form), `import module data as Data;` and `import external shared as S;`
  are the syntax for cross-module and remote dependencies, and `export`
  marks declarations visible to importers. The `import module/external`
  forms and export visibility are parsed and specified but rejected by the
  analyzer until the linking milestone lands — the examples in
  `docs/examples/` (notably "Multi Module" and "External Module") are the
  spec for that work.
- **`class` / `new` remain reserved, still unimplemented.** v1's lexer
  already reserves these keywords for a future OOP extension. v2 keeps them
  reserved (they cannot be used as identifiers) but defines no grammar for
  them; out of scope for this pass.
- **Flat files: a file is a type, `export` is removed.** Superseding the
  point above — classes arrive not as a `class Foo { }` wrapper but as a
  dedicated file kind (`.cpirate`/`.cpir`), where the filename *is* the type
  name and every top-level member belongs to it (GRAMMAR.md §4). This
  reuses v2's existing "declarations at top level" shape for modules rather
  than adding a second, wrapper-based declaration style. `class` stays
  reserved-but-unused (the extension carries the kind, not a keyword); `new`
  is promoted to an expression keyword, used only to create an instance —
  never to introduce a type. `export` (3.3 in the previous revision of this
  doc) is dropped everywhere, including plain modules: one visibility rule,
  `private` opts a declaration out of the public-by-default rule, for both
  class members and top-level module declarations. See
  [`brainstorm/FLAT.md`](brainstorm/FLAT.md) for the full rationale
  (including the "why not `member`" and "why not an accessible/mutable
  keyword pair" discussions) and
  [`brainstorm/FLAT_PLAN.md`](brainstorm/FLAT_PLAN.md) for the phased build
  order this unlocks (module linking, nullable scalars in the VM, etc.).
