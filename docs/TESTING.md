# Pirate v2 Testing Strategy

Status: **canonical** for `src-v2/`. Two layers, each with a distinct job:

1. **Unit tests (xUnit)** — one test project per library project, verifying
   that component in isolation.
2. **End-to-end tests (Gherkin/Reqnroll)** — black-box scenarios that feed a
   real `.pirate` script through the full pipeline (lexer → parser →
   semantics → compiler → VM) and assert on the actual output, the same way
   a user would run `pirate run`.

Unit tests catch "does this function do the right thing"; e2e tests catch
"does the compiler as a whole produce the right program". Both are required
— neither substitutes for the other. A change to the grammar
([`GRAMMAR.md`](GRAMMAR.md)) is not done until both layers reflect it.

## 1. Unit tests

Every project in `src-v2/PirateLang.slnx` is tested with xUnit where the
project has behavior to unit-test:

| Project | Test project | What it verifies |
|---|---|---|
| `Pirate.Syntax` | *(none — plain data types)* | AST records have no logic; covered indirectly by every other project's tests. |
| `Pirate.Lexer` | `Pirate.Lexer.Test` | Source text → correct token stream, including line/column tracking and error cases (unterminated string, unknown character). |
| `Pirate.Parser` | `Pirate.Parser.Test` | Token stream → correct AST per [`GRAMMAR.md`](GRAMMAR.md) §3, and correct diagnostics (not exceptions) on malformed input. |
| `Pirate.Semantics` | `Pirate.Semantics.Test` | Name resolution and the static-typing rules in `GRAMMAR.md` §2 — valid programs pass, each documented type error is caught with the right message. |
| `Pirate.Compiler` | `Pirate.Compiler.Test` | Checked AST → correct bytecode (opcodes, constant pool, jump targets) for each construct. |
| `Pirate.VM` | `Pirate.VM.Test` | Given a hand-built bytecode chunk, the stack machine produces the correct result/state — independent of whatever the compiler emits, so a VM bug and a compiler bug can't mask each other. |
| `Pirate.StandardLibrary` | `Pirate.StandardLibrary.Test` | Each native function (`Standard.Terminal.Print`, `Standard.String.Length`, …) against its inputs/outputs and error cases, independent of the VM. |
| `Pirate.Cli` | *(no dedicated xUnit project)* | Command wiring is thin (Spectre.Console.Cli settings → pipeline calls); real coverage comes from the e2e layer below, which exercises the CLI as a whole. |

Stack: **xUnit** + **AutoFixture** (anonymous test data) + **FakeItEasy**
(fakes for collaborators), matching v1's test stack. One behavior per
`[Fact]`/`[Theory]`; arrange/act/assert, no shared mutable fixture state
between tests unless the type under test is genuinely stateless.

Rule of thumb for what's worth a unit test: anything with a decision, a
calculation, or an edge case. Passthrough plumbing isn't.

## 2. End-to-end tests (Gherkin)

Project: `Pirate.Spec.Test`, using **Reqnroll.xUnit** (SpecFlow's maintained
successor — v1 used SpecFlow.xUnit, which is EOL) and **FluentAssertions**.

Each scenario is a `.pirate` script plus the exact result it must produce.
The script is real Pirate source, written inline as a docstring so the
scenario is self-contained and readable without opening another file:

```gherkin
Feature: If statement

Scenario: True condition runs the then-branch
    Given the following pirate program:
        """
        extern Standard.Terminal.Print;

        func main() : void
        {
            if 3 == 3
            {
                Print("Ahoy!");
            }
        }
        """
    When the program is run
    Then stdout should be:
        """
        Ahoy!
        """
```

Step definitions (`StepDefinitions/`):
- **Given** writes the docstring to a temp `.pirate` file.
- **When** invokes the v2 pipeline exactly as `Pirate.Cli`'s `run` command
  does (lex → parse → check → compile → execute), capturing stdout/stderr
  and the exit code instead of talking to the real console.
- **Then** asserts the captured output/exit code with FluentAssertions —
  **this step must actually assert**. v1's equivalent step
  (`src/Pirate.Spec.Test/StepDefinitions/CommonSteps.cs`) only printed the
  result to the console and never called `.Should()`, so its scenarios could
  never fail no matter what the interpreter produced. That is the one
  concrete mistake this layer exists to not repeat.

### Coverage target

Every construct in `GRAMMAR.md` §3 (Grammar) gets at least one scenario:
variable declaration (`var` and explicit type), assignment, `if`/`else`,
`while`, both `for` forms (counting and `for (x in list)`), array literals
and indexing, function declaration/call/return, and `extern`. Add a scenario
alongside any grammar change in the same PR — the grammar and its e2e
coverage move together.

Unit tests are added per-project as that project is built; the e2e suite is
the parity gate before v1 is retired (see the v2 migration plan) — every
`.pirate` sample that runs on v1 must run on v2 with matching output before
the cutover.
