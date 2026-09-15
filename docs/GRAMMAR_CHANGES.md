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
- **`class` / `new` remain reserved, still unimplemented.** v1's lexer
  already reserves these keywords for a future OOP extension. v2 keeps them
  reserved (they cannot be used as identifiers) but defines no grammar for
  them; out of scope for this pass.
