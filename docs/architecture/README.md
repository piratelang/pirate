# Architecture overview

This repo contains two complete, independent implementations of the Pirate
language, in two separate solutions that share no projects:

- [`v1-architecture.md`](v1-architecture.md) — `src/PirateLang.sln`, the
  currently-shipped implementation.
- [`v2-architecture.md`](v2-architecture.md) — `src-v2/PirateLang.slnx`, the
  ground-up rewrite in progress: Spectre.Console CLI, a faster lexer/parser,
  and a compiled (bytecode + VM) pipeline with static typing, replacing v1's
  tree-walking interpreter.

Read both before touching either solution — v2 is not a port of v1's code,
but it is a replacement for v1's *behavior*, so knowing what v1 actually does
(not just what its docs claim) matters when v2 has to match or deliberately
diverge from it.

Related docs, not architecture but load-bearing for the same work:

- [`../GRAMMAR.md`](../GRAMMAR.md) — canonical v2 language grammar.
- [`../GRAMMAR_CHANGES.md`](../GRAMMAR_CHANGES.md) — why v2's grammar differs from v1.
- [`../TESTING.md`](../TESTING.md) — testing strategy for both layers (xUnit + Gherkin).

## Current state

v1 is the shipped version and must keep working. v2 is scaffolded
(`src-v2/PirateLang.slnx` exists with empty/stub projects wired together per
`v2-architecture.md`) but has no lexer/parser/compiler/VM logic yet. There is
no migration/cutover doc checked in yet — treat "when/how v1 gets deleted" as
undecided until the user says otherwise.
