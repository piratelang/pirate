# Pirate v2 examples

These folders define the syntax of Pirate v2 — they are the canonical
usage examples for [`docs/GRAMMAR.md`](../GRAMMAR.md), mirrored by tests
(`Pirate.Cli.Test/Services/ExampleProjectsTests.cs` compiles each program
through the real frontend).

Each folder is a complete project: a `.fleet` manifest plus `.pirate`
modules. To work with one:

```
cd "docs/examples/1 - Hello World"
pirate build      # type-checks every module (cache makes repeats no-ops)
pirate run        # runs the manifest's entryPoint module's top-level code
```

(Until the VM lands, `pirate run` type-checks and then reports that
execution is not implemented yet — `pirate build` does all the checking.)

## The tour

| Folder | Shows |
|---|---|
| **1 - Hello World** | The smallest runnable program: `import standard` + a top-level call. No `main()` — the entry module's statements *are* the program (§3.1). |
| **2 - More Operations** | The §2–§3 surface: all three declaration spellings (`var`, `<type>`, `const`), arrays, empty literals needing element types, string concatenation, if/else-if chains, counting `for`, `for-in`, `while`, function declarations called from top level, and dotted builtin paths. |
| **4 - Multi Module** | The module system as specified — public-by-default declarations, `import module data as Data;`, reaching them through the alias. |
| **5 - External Module** | The external-dependency shape — `import external data as Data;` and what `dependencies` will eventually declare. |
| **6 - Classes** | The flat-files classes first slice (GRAMMAR.md §4) — a `.cpirate` class file (`Counter`: fields, constructors, `readonly`/`private`, methods, `self`-free member access), same-folder visibility with a plain module, and `new`. |

## Status honesty

Examples 1 and 2 compile clean through the frontend today.
Examples 4 and 5 are **spec examples**: they parse (the grammar is settled
and tested for them), but the semantics pass reports
`SEM-013 — Module import not supported yet`, because cross-module linking
— resolving `import module`/`import external`, enforcing visibility, and
requiring imported helper modules to stay declaration-only (GRAMMAR.md
§3.1–3.3) — is Phase 3 of the flat-files plan
([`brainstorm/FLAT_PLAN.md`](../brainstorm/FLAT_PLAN.md)). They were
previously written against `fleet.json` manifests with
`modules[]`/`external[]` file lists; `[v2-021]` in
`docs/design/DESIGN_DECISIONS.md` records the move to `.fleet` and the
deferred linker, and [`../FLEET.md`](../FLEET.md) the manifest schema that
linking will extend. `export` (previously used by example 4) was removed —
`[v2-030]` in [`../design/DESIGN_DECISIONS.md`](../design/DESIGN_DECISIONS.md)
— every declaration is public by default now.

Example 6 is a **spec example** one milestone further out than 4/5: class
files don't lex or parse yet at all (no `field`/`constructor`/`self`
keywords, no `.cpirate` file-kind handling), so it will not compile through
today's frontend — it exists so `GRAMMAR.md` §4's class-file grammar has a
worked program to point at, per the "examples are the spec" decision.

Numbering note: there is no "3" — kept as-is intentionally; the numbers
mark the tour order, not a count.
