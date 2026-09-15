# Pirate — instructions for agents working in this repo

## Required reading, before any change

Read **every file in [`docs/architecture/`](../docs/architecture/)** in
full before starting work — not just the file that looks most relevant.
That folder is the source of truth for how v1 and v2 are actually built,
not a summary; it currently contains:

- [`docs/architecture/README.md`](../docs/architecture/README.md) — how v1 and v2 relate, and current project state.
- [`docs/architecture/v1-architecture.md`](../docs/architecture/v1-architecture.md) — v1's actual pipeline, file-by-file.
- [`docs/architecture/v2-architecture.md`](../docs/architecture/v2-architecture.md) — v2's pipeline, project graph, and design decisions.

If a new file is added to `docs/architecture/` after this instruction was
written, read that one too — "the entire folder" means whatever is in it at
the time, not just the three above.

Also read, as needed for the task:

- [`docs/GRAMMAR.md`](../docs/GRAMMAR.md) — canonical v2 language grammar. Any change to v2 syntax or semantics must be reflected here in the same change.
- [`docs/GRAMMAR_CHANGES.md`](../docs/GRAMMAR_CHANGES.md) — why v2 differs from v1. Read before assuming v1 behavior carries over.
- [`docs/TESTING.md`](../docs/TESTING.md) — testing strategy: xUnit per project, Gherkin/Reqnroll end-to-end. Any grammar or pipeline change needs unit tests in the matching `*.Test` project and, for syntax/semantics changes, an e2e scenario in `Pirate.Spec.Test`.

Root `GRAMMAR.md` and `SYNTAX.md` (outside `docs/`) describe **v1 only** and
are historical — do not use them as a source of truth for `src-v2/` work,
and do not edit them as part of v2 work.

## Two solutions, kept independent

- **v1** — `src/PirateLang.sln`. Currently shipped; do not break it while v2
  is in progress.
- **v2** — `src-v2/PirateLang.slnx`. Rewrite in progress; see
  `v2-architecture.md` for what exists vs. what's still a stub.

Do not add cross-references between `src/` and `src-v2/` projects.

## Before declaring any task finished

Run the test suite for whichever solution(s) the change touches, and don't
report the task as done until it actually passes (or you've explained, with
specifics, why a failure is pre-existing/unrelated — not assumed away):

```
dotnet test src/PirateLang.sln         # any change under src/
dotnet test src-v2/PirateLang.slnx     # any change under src-v2/
```

If the change touched both, or touched shared docs whose claims cover both
(e.g. `docs/TESTING.md`), run both. This is not optional and not satisfied
by the code merely compiling — actually run `dotnet test` and read the
result before saying you're finished.
