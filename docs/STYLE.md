# Pirate v2 — Code Style Guide

This guide governs **`src-v2/` only**. Every rule below is how v2 code is
written and how all new code must be written; existing code that deviates is
non-compliant and listed in the [Migration appendix](#migration-appendix) as
follow-up work, not precedent. v1 (`src/`) has no style guidance here — it is
the shipped legacy implementation and is not a model to follow or a surface to
"modernize".

## Languages and where they go

- **C#** for everything: lexer, syntax/AST, semantics, compiler, VM,
  standard library, shared libraries, CLI. `net9.0`, `Nullable` enable.
- **F#** is retained where it earns its place — currently the parser
  (`Pirate.Parser`), whose Pratt/pattern-matching shape is a natural fit.
  F# stays v2-native:
  - `module`s of `let`-bound functions, not class-emulating `member this.`
    code; `internal` visibility by default, `module Parser` as the public
    seam.
  - Discriminated unions and pattern matching over enums-plus-casts where
    the domain is genuinely variant.
  - Mutable state, when needed, is one explicit state record passed through
    the functions (`ParserResult.fs`'s `State` record with `Pos`/`Tokens`/`Errors`) — never
    ambient `let mutable` fields.
  - `.fsproj` lists `<Compile Include>` in explicit dependency order;
    `ImplicitUsings` is disabled for F# projects.
- Both languages interop through `Pirate.Syntax` types; an F# project and a
  C# project never define parallel versions of the same concept (v1's
  duplicate C#/F# `TokenType` plus mapper bridge is the anti-pattern to
  avoid).

## Structure

- One type per file, named after the type. `Pirate.Syntax/Nodes/` is one
  node type per file; base records shared by them live together in
  `BaseNodes.cs`.
- The namespace always matches the project name.
- Folder types: `Nodes/`, `Services/`, `Commands/` — a folder exists when
  there are three or more related types to justify it, otherwise types sit
  at project root.
- Any capability needed by **two or more projects** moves into a
  `Pirate.Shared.*` project (`Pirate.Shared.File`, `Pirate.Shared.Logging`,
  …) rather than being duplicated or reaching across the graph. Shared
  projects stay dependency-free (BCL only) plus the DI abstractions noted
  below.

## Composition — everything injectable, per-project extensions

v2 is a dependency-injected solution end to end. No stage is consumed
through static calls or hand-wired `new` chains; every project registers its
own services through an extension method on `IServiceCollection`:

```csharp
namespace Pirate.Lexer;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPirateLexer(this IServiceCollection services)
    {
        services.AddSingleton<ILexer, Lexer>();
        return services;
    }
}
```

- **Pipeline stages are services behind interfaces** — `ILexer`, `IParser`,
  `ISemanticAnalyzer`, `ICompiler`, `IVM`, … — each with a single
  implementation registered by its own project's extension.
- Each project references only
  `Microsoft.Extensions.DependencyInjection.Abstractions` (for
  `IServiceCollection` and `AddSingleton`/`AddScoped`/`AddTransient`); the
  concrete container is owned by `Pirate.Cli` alone.
- Lifetime choice: stateless stages and pure helpers → `AddSingleton`;
  per-run state (`BuildCache`, analysis sessions) → `AddScoped`;
  short-lived workers → `AddTransient`.
- `Pirate.Cli`'s `Program.cs` composes the app **exclusively** by calling
  the projects' `Add*()` extensions and then resolving; commands receive
  their dependencies via constructor injection (Spectre.Console.Cli
  supplies the container to `Command<TSettings>` instances). A command
  never news up a pipeline stage or calls a static entry point directly.
- Test code constructs implementations directly or builds a minimal
  `ServiceCollection` with the same extensions — the seams are identical.

## Naming

- **No shortened names, anywhere.** Write every word in full:
  `LeftParenthesis` (not `LeftParen`), `Statement` (not `Stmt`),
  `Expression` (not `Expr`), `Result` (not `res`), `Context` (not `ctx`),
  `Token` (not `tok`). This binds on declarations — types, members,
  parameters, locals, enum values — and new code may not shorten even when
  the neighboring existing name is short.
- Enum members are PascalCase (`TokenType.Plus`, `LexErrorKind.UnterminatedStringLiteral`),
  never SCREAMING_SNAKE; private fields are `_camelCase`; parameters and
  public members are PascalCase.
- AST types keep the `*Node` suffix and the conceptual names shared across
  the pipeline (`BinaryOperationNode`), as positional `sealed record`s whose
  first two parameters are always `StartLocation`, `EndLocation`.
- Command settings are nested inside their command:
  `RunCommand.RunCommandSettings : GlobalSettings`.
- **Fully qualify** any name that could bind ambiguously — C# resolves
  enclosing namespace segments before `using` aliases, so in `Pirate.*`
  contexts write `Pirate.Lexer.Lexer.Tokenize(...)`,
  `Pirate.Parser.Parser.Parse(...)`, and `System.IO.File.ReadAllText(...)`
  (inside `Pirate.Shared.File`, `File` alone means the namespace, not the
  BCL type). When in doubt, qualify; a qualification is never wrong.

## Diagnostics and error handling

- Pipeline stages **never throw for user errors**. Each stage returns its
  own typed, collected result (`LexResult`, `ParseResult`,
  `SemanticResult`) carrying `CompilationError` subclasses with an
  `*ErrorKind` enum value and `SourceLocation`s.
- Each stage reports only its own errors; the CLI concatenates and renders
  them (one stage must not re-emit an upstream stage's errors).
- Error codes (`LEX-xxx`, `SYN-xxx`, `SEM-xxx`) exist **only** in
  `Pirate.Cli`'s `ErrorMapper`. Libraries are code-agnostic.
- Throwing is reserved for genuine faults (bug invariants, broken
  preconditions) — `ArgumentNullException.ThrowIfNull` for arguments, a
  normal exception for a corrupted internal state.
- Every token and every node carries real source locations. Nothing in v2
  mutates or strips the input text before scanning.

## Comments

- Public types and members get an XML `<summary>` that explains **what the
  thing does in v2, self-contained** — its role in the pipeline, its
  invariants, its failure modes. No "unlike v1 …", "v1 did X, we do Y"
  framing: v2 comments must read correctly to someone who has never seen v1.
- References to the spec are kept and welcome: `docs/GRAMMAR.md §3.5` style
  citations say *what the code must do* and stay the authority for parser
  and grammar behavior.
- Inline comments explain *why* at non-obvious sites: language gotchas
  (namespace-shadowing qualifications above), platform quirks
  (`Console.OutputEncoding`), and deliberate safety choices (treating a
  corrupt build cache as empty). They never just restate the code.
- `docs/GRAMMAR_CHANGES.md` is where v1-to-v2 comparisons belong; code does
  not carry them.

## Modern C# idiom (`net9`)

- File-scoped namespaces (`namespace X;` at the top of the file), never block-bodied.
- Collection expressions (`= []`, `[TokenType.Eof]`) and target-typed `new`.
- Pattern-based `switch` with `case ' ' or '\t'` or-patterns and `when`
  guards, over cascaded `if`s.
- Expression-bodied members for one-liners (`private bool IsAtEnd => _position >= _source.Length;`);
  expression-bodied methods when the body is a single call or `return`.
- `sealed` by default on implementation classes and records; abstract bases
  are `abstract record` when they are data.
- No `Console.Write*` in the CLI — everything goes through
  `Spectre.Console` (`AnsiConsole`), themed via `Pirate.Cli/Theme.cs`.

## Testing

- xUnit is the base; **libraries are used where they add clarity** —
  FakeItEasy for standing up interface dependencies in unit tests,
  AutoFixture for anonymous data, FluentAssertions for readable compound
  assertions. A test that reads better with real inputs and structural
  assertions should not be mocked just to mock.
- Shared test features are extracted when duplicated across suites: common
  fixtures (a `Helpers` module in F# suites, shared source-wrapping or
  temp-directory helpers) move into a `*.Test`-local `Helpers` class or, if
  needed by two test projects, a shared test project — don't copy-paste
  fixture code.
- Naming: C# uses `Method_Scenario_Result` (`Tokenize_Keyword_ProducesKeywordToken`);
  F# uses double-backtick descriptive test names (`` `Operator precedence — multiplication before addition` ``).
- `[Theory]` + `[InlineData]` for any table-shaped set of cases; one
  behavior per test.
- Data-only projects (e.g. `Pirate.Syntax`) have no test project; anything
  with logic does, named `<Project>.Test`.
- End-to-end behavior is Gherkin/Reqnroll in `Pirate.Spec.Test`, asserting
  actual output — a `Then` step must always assert.

## Build conventions

- Solution file format is `.slnx` (`src-v2/PirateLang.slnx`).
- All projects target `net9.0` with `Nullable` enable; C# projects enable
  `ImplicitUsings`, F# projects disable it.
- Project references follow the pipeline direction defined in
  `docs/architecture/v2-architecture.md`; no project references upward
  toward `Pirate.Cli`, and nothing under `src-v2/` references anything under
  `src/`.

## Migration appendix

The rules above are the standard; the existing code is not yet at that
standard. The following deviations are tracked as separate follow-up work
items — explicitly **not** part of adopting this document, and none of them
should be treated as precedent when writing new code:

- **(a) Rename shortened names.** `Pirate.Lexer.TokenType` still contains
  `LeftParen`/`RightParen` (and the parser/CLI/tests that use them), plus
  any other abbreviated identifiers found by review. Renames must update
  code, tests, `docs/GRAMMAR.md`, and error catalogs in the same change.
- **(b) Convert static stages to injected services.** `Pirate.Lexer.Lexer.Tokenize`
  and the F# `Parser.Parse` entry points are still static, and `Pirate.Cli`
  commands call them directly. Each needs an interface (`ILexer`, `IParser`),
  an implementation class, a per-project `ServiceCollectionExtensions`
  (`AddPirateLexer()`, `AddPirateParser()`, …), and CLI commands switched to
  constructor injection — including the existing `Add*()`-less composition in
  `Program.cs`.
- **(c) Strip v1 cross-references from v2 comments.** Doc comments in at
  least `Pirate.Lexer/Lexer.cs`, `Token.cs`, `TokenType.cs`,
  `Pirate.Shared.File/PirateFileLocator.cs`, `PirateFileName.cs`,
  `Pirate.Cli` (`Banner.cs`, `NewCommand.cs`, `InitCommand.cs`), and
  `Pirate.Syntax/Nodes/WhileStatementNode.cs` currently explain v2 by
  contrast with v1. They must be rewritten to stand alone.

Each item should land as its own change (a and c touch docs and tests they
must keep in sync; b is a pipeline-wide interface seam) and be recorded in
`docs/internal/DESIGN_DECISIONS.md` when implemented.
