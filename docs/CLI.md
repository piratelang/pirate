# Pirate v2 CLI

Status: **canonical** for `Pirate.Cli` (`src-v2/Pirate.Cli`). Documents the
command structure, what each command actually does today, and how it maps to
v1's `Shell` project. See
[`docs/architecture/v2-architecture.md`](architecture/v2-architecture.md) for
where the CLI sits in the overall pipeline, and [`docs/FLEET.md`](FLEET.md)
for the `.fleet` project manifest `init`/`run` use below.

## Framework

Built on **Spectre.Console.Cli** (`CommandApp`), replacing v1's hand-rolled
`CommandManager`/`CommandFactory`/`ICommand` dispatch
(`src/Shell/CommandManager.cs`,
`src/Shell/Commands/CommandFactory.cs`). Each command is a
`Spectre.Console.Cli.Command`/`Command<TSettings>` with `[CommandArgument]`-
declared positional arguments — argument parsing, `-h`/`--help` generation,
and usage/examples text come from Spectre, not hand-written per command like
v1's `Help()` overrides and manual `args.Contains("-h")` check.

Command surface is unchanged from v1: `run`, `init`, `new`, `build`, `shell`,
plus the no-args banner. Every command still returns a process exit code
(`0` success, non-zero failure) the same way v1's commands did via `Error()`.

Every command's settings class inherits `GlobalSettings` — the seam any
flag or behavior meant to apply to every command hangs off, rather than
adding it to `RunCommandSettings`/`InitCommandSettings`/etc. individually.
It currently carries the cross-cutting `-v|--verbose` option, consumed by
`build` to print each module's content hash (for both rebuilt and
up-to-date files); future global flags extend this class, not the
per-command ones.

## Commands

### No arguments

`pirate` with no arguments prints a banner and command list — reproducing
v1's exact `NoCommand`/`Application.Run` behavior (banner + command list on
no args) rather than Spectre's default auto-generated help, but built from
real Spectre.Console renderables instead of v1's hand-joined ASCII-art
string: `FigletText` for the "Pirate" title, `Rule` as a section divider, and
a borderless `Table` for the command list (`Banner.cs`). This path bypasses
`CommandApp` entirely (`Program.cs` checks `args.Length == 0` before
constructing it) since there's no command to route to.

`Banner.Commands` is a plain data list (usage, description) — kept separate
from the rendering so it's unit-testable without a console. The rendering
itself (`Banner.Render`) *is* tested, via `Spectre.Console.Testing`'s
`TestConsole` (swap it in for `AnsiConsole.Console`, render, assert on
`console.Output`) — this is what would have caught the `[type]`-parsed-as-
style-tag bug below automatically, since it actually exercises markup
parsing instead of just checking string contents.

### `pirate init [filename] [-n|--name]`

Creates a new `.pirate` file from the hello-world template. `filename`
defaults to `main`; the `.pirate` extension is optional (accepted with or
without it). The template follows [`docs/GRAMMAR.md`](GRAMMAR.md) — a
standard-library import plus a top-level call, since the entry module's
statements are the program (GRAMMAR.md §3.1):

```pirate
import standard Terminal;

PrintLine("Hello World");
```

`init` also writes a [`.fleet`](FLEET.md) manifest alongside it — `name`
from the current directory's folder name, `entryPoint` set to whatever
`filename` resolved to. The manifest's own base file name defaults to
`module` (i.e. `module.fleet`), overridable with `-n|--name`, the same
resolve-with-a-default shape `[filename]` already has for the `.pirate`
file (`FleetFileName.Resolve`, mirroring `PirateFileName.Resolve`). Unlike
the `.pirate` file (always overwritten, unchanged behavior from before
`.fleet` existed), a `.fleet` write is guarded: `init` refuses and exits
`1` if a `*.fleet` already exists in the current directory under any name,
rather than clobbering a project's existing metadata.

Examples:

```
$ pirate init

Created main.pirate
Created module.fleet

$ pirate init sample -n my-project

Created sample.pirate
Created my-project.fleet

$ pirate init

Created main.pirate
A ".fleet" file already exists
```

That last run's `main.pirate` line shows the pre-existing, unguarded
overwrite behavior — `init` happily rewrote `main.pirate` again before
hitting the `.fleet` guard and exiting `1`. This gap predates `.fleet` and
isn't addressed here.

### `pirate new [type] [filename]`

Creates a file from a template, same three types as v1: `pirate` (empty
module, `filename` defaults to `main`, fails if the file already exists),
`gitignore`, `gitattributes`. `gitignore`/`gitattributes` also refuse to
overwrite a file that already exists, matching the `pirate` case, rather
than silently clobbering a customized one.

`NewFileTypes` (the list of valid types) and the `IsValidNewFileType` check
live as private members of `NewCommand` itself, not in the shared
`Templates` service — nothing outside `NewCommand` needs them.

Running `pirate new` with no type, or `pirate new list` explicitly, prints
the options with the same look as the no-args banner's command list
(`Banner.cs`) — a `Rule` divider over a borderless, bold-row `Table` —
rather than a plain bulleted `WriteLine` list. An unrecognized
type is rejected by `NewCommandSettings.Validate()` before `Execute` runs —
Spectre's own validation error, not a manual check — so it's always an
error, never a silent no-op; `Validate()` treats `list` the same as no type
at all (a legitimate request to see the options, not a validation failure).

Examples:

```
$ pirate new

The "pirate new [type]" command creates a new file from a template

── Options ──────────────────────────────────────
pirate
gitignore
gitattributes

$ pirate new pirate sample

Created sample.pirate

$ pirate new gitignore
Specified filename ".gitignore" already exists

$ pirate new bogus
Error: Specified file "bogus" not able to be created
```

### `pirate build [filename]`

With no `filename`, discovers every `*.pirate` file under the current
directory recursively; with one, resolves and builds just that file (same
resolution as `run`). Every discovered module runs through the full
frontend — `CompilationPipeline`: lexer → parser → semantics — and all
errors from every stage are rendered together per file with source
excerpts and `LEX`/`SYN`/`SEM` codes (`DiagnosticRenderer` +
`ErrorMapper`).

Builds are incremental by **content hash** (`BuildCache`, `.pirate/cache.json`,
SHA-256 per file): an unchanged module is reported `(up to date)` and not
recompiled; a changed one is recompiled and reported `(rebuilt)`. The cache
is also stamped with the front-end's `PipelineVersion` — a cache written by
an older pipeline (one that checked less than today's does) is discarded
whole on load, so "(up to date)" always means "clean under the compiler you
just ran" (bump the version whenever a build becomes meaningful differently). A module
is only marked built after a fully clean frontend pass — a file with type
errors never gets cached as good, so fixing it forces a rebuild and an
uncached bad file keeps re-reporting. A corrupt or unreadable cache is
treated as empty (rebuild everything) rather than fatal, and saves are
temp-file + rename so a crash can't write a half-cache. Exit code is `1`
if any module failed, `0` otherwise.

Examples (real output):

```
$ pirate build
── Build ──────────────────────────────────────────
  ⟳ main.pirate (rebuilt)

1 module: 1 rebuilt, 0 up to date

$ pirate build bad
── Build ──────────────────────────────────────────
  ✗ bad.pirate (failed)
bad.pirate:3:1 Cannot assign to 'LIMIT' (declared 'const') *SEM-004*
  2 |     const int LIMIT = 10;
  3 |     LIMIT = 20;
    |     ^^^^^

1 module: 0 rebuilt, 0 up to date, 1 failed

$ pirate build nope
File "nope.pirate" not provided or does not exist.
```

### `pirate run [filename]`

Resolves `filename` and checks the `.pirate` file exists. Resolution order
with **no** `filename` goes through [`.fleet`](FLEET.md)
(`Pirate.Fleet`'s `FleetEntryPoint`): whatever `*.fleet` file
`FleetFileLocator` finds in the current directory supplies its
`entryPoint`, falling back to `main`. A `.fleet` that exists but isn't
valid JSON is reported clearly and exits `1`. With an explicit
`filename`, resolution accepts the name with or without the extension,
and the argument always beats the manifest.

What `run` does today is real up to the last step: the entry module runs
through the full frontend (`CompilationPipeline`) — and, unlike `build`,
**always**, even when the cache says the module is up to date, because
there is no compiled artifact to reuse yet and the entry check needs the
checked AST. The cache still decides whether the Build phase prints
`(rebuilt)` or `(up to date)`. Only modules that pass lex+parse+semantics
proceed; all frontend errors render as for `build`.

The entry rule (GRAMMAR.md §3.1) is checked at run time, in `Pirate.Cli`
(`Services/EntryPoint`), not in the type-checker: the entry module must
carry top-level statements — that statement list *is* the program. A
declaration-only module is a valid, buildable module with nothing to run:

```
$ pirate run decl-only
── Build ──────────────────────────────────────────
  ⟳ decl-only.pirate (rebuilt)
── Running ───────────────────────────────────────
Module 'decl-only.pirate' has no top-level statements — nothing to run.
```

When the module does have a runnable body, `run` is honest about the
remaining stub:

```
$ pirate run
── Build ──────────────────────────────────────────
  ✓ main.pirate (up to date)
── Running ───────────────────────────────────────
✓ main.pirate type-checked clean.
Execution is not implemented yet — the v2 VM pipeline is still a stub.
```

The whole resolve/build/run sequence runs inside an `AnsiConsole.Status()`
spinner; the "Execution is not implemented yet" line is the seam where
compile + VM execution (and RTN-004 for the nothing-to-run case) will
hang once the pipeline's back half exists, so `run` won't need
restructuring to do real work later.

### `pirate shell`

Opens a read-eval-print loop: prints the version banner, then reads lines
from stdin until `stop`, `exit`, or `break` or EOF. Uses plain
`Console.ReadLine()`, not a Spectre `TextPrompt` — a `TextPrompt` requires
an interactive terminal and fails on piped/redirected stdin, which would
break both scripted usage and any future e2e test that feeds a script into
`pirate shell` via stdin. Each line is a fresh, line-numbered source fed
through the full frontend (`CompilationPipeline`): every lex, syntax, and
semantic error renders with `file:line:col`, the offending line, and a
caret; a clean line reports the type-check success and the same execution
stub as `run`:

```
>> import standard Terminal;
(type-checked clean, but execution is not implemented yet)
>> func f(void x) : int { return 1; }
stdin:2:8 'void' is not a value type *SEM-003*
  2 | func f(void x) : int { return 1; }
             ^
```

## Error handling

`Program.cs` registers a `CommandApp`-level `SetExceptionHandler`, which
takes over *all* exception handling — including Spectre's own for
parse/validation errors (`Spectre.Console.Cli.CommandAppException` and its
`CommandParseException`/`CommandRuntimeException` subclasses, e.g. what
`NewCommandSettings.Validate()` throws on an unknown `[type]`). The handler
tells the two cases apart instead of relabeling an expected validation
message as a bug:

- **`CommandAppException`** (expected, user-facing CLI errors): rendered
  plainly as `Error: <message>` (or via the exception's own `Pretty`
  renderable when one is attached) — no "unexpected error" framing, no
  stack trace.
- **Anything else** (a genuine unhandled fault): printed as
  `[red]Unexpected error: <message>[/]` (message escaped), no stack trace.

Both paths exit with `-1`. This exists mainly for when `run`/`shell` start
executing real VM code and a runtime fault becomes possible; today's stub
commands only throw via `Validate()`.

## What's deliberately different from v1

- Argument parsing, help text, and validation are declarative
  (`CommandSettings` + `[CommandArgument]`) instead of each command manually
  indexing into `string[] arguments`. Settings classes are nested inside the
  command they belong to (`NewCommand.NewCommandSettings`, not a top-level
  sibling class), and cross-cutting checks use Spectre's own
  `CommandSettings.Validate()` hook (`NewCommand.NewCommandSettings.Validate()`
  rejects an unknown `[type]` before `Execute` ever runs) instead of a manual
  `if` at the top of `Execute`.
- Colors are centralized in `Theme.cs` (`Theme.Error`/`Warning`/`Success`/
  `Info`/`Accent`) rather than each command hardcoding its own markup tag
  names, so every command agrees on what "error"/"warning" look like.
- Output goes through `Spectre.Console.AnsiConsole` (`MarkupLine`, `Table`)
  instead of raw `Console.Write` + manual `ConsoleColor` swaps — with one
  care point: any interpolated value that isn't a trusted literal (a
  filename, a type argument) is passed through `Markup.Escape(...)` before
  being embedded in a markup string, and any literal text containing `[` /
  `]` that isn't meant as a style tag (e.g. the `new` command's usage text
  showing `[type]`) is printed via `AnsiConsole.WriteLine` instead of
  `MarkupLine` — Spectre otherwise parses `[...]` as a style tag and throws
  `Could not find color or style '...'` on unescaped brackets. This is a real
  bug class this CLI hit once already (see git history / tests); don't
  reintroduce it by adding a new `MarkupLine` call with an unescaped
  interpolated value.
- The version string comes from the `Version` MSBuild property in
  `Pirate.Cli.csproj` (read at runtime via
  `AssemblyInformationalVersionAttribute`), not a hardcoded constant — bump
  the version in one place. `IncludeSourceRevisionInInformationalVersion` is
  set to `false` so the displayed version doesn't get a `+<git-sha>` suffix
  appended by the SDK's deterministic-build feature. The reflection lookup
  is shared through `CliInfo.Version()` (`CliInfo.cs`), used by `Banner`
  and `ShellCommand` — one source of truth and one lookup.
- `build`/`run`/`shell` are honest about pipeline status: the whole
  frontend they can do today (lexing, parsing, type-checking, diagnostics)
  is real, and execution prints an explicit "not implemented yet" because
  the compiler/VM half of the pipeline doesn't exist — rather than a
  silent no-op or a fake success.
- `pirate shell` reads input with plain `Console.ReadLine()`, not a Spectre
  `TextPrompt` — a `TextPrompt` requires an interactive terminal and throws
  on piped/redirected stdin, which would break both scripted usage and any
  future e2e test that feeds a script into `pirate shell` via stdin.
- `Program.cs` sets `Console.OutputEncoding = Encoding.UTF8` at startup
  (wrapped in try/catch — redirected output can reject this on some
  platforms). Windows consoles don't default to UTF-8, which otherwise
  mangles any Unicode glyphs Spectre renders (`Rule`'s divider line, rounded
  `Table` borders, box-drawing characters generally) into `?`/`�`. This bit
  the banner's `Rule` during development. **`BuildCommand`'s table used to
  sidestep this** by forcing `TableBorder.Ascii` regardless of encoding — it
  now matches `Banner`/`NewCommand`'s `Rule`-plus-borderless-`Table` look
  instead, for visual consistency across commands, which reintroduces the
  same Unicode-mangling exposure to `build`'s output (its `Table` itself is
  borderless, so no border characters to mangle either way, but the `Rule`
  divider above it carries the same risk `Console.OutputEncoding = UTF8` is
  there to prevent). Consistency won out over that isolation for now.

## Project layout

```
Pirate.Cli/
  Program.cs                    entry point, CommandApp configuration, no-args banner
  Banner.cs                     ASCII art + command list (no-args output)
  GlobalSettings.cs             base settings type every command's settings inherits (carries -v|--verbose)
  Theme.cs                      shared markup colors (error/warning/success/info/accent)
  Commands/
    InitCommand.cs               settings nested as InitCommand.InitCommandSettings
    NewCommand.cs                settings nested as NewCommand.NewCommandSettings
    BuildCommand.cs              settings nested as BuildCommand.BuildCommandSettings
    RunCommand.cs                settings nested as RunCommand.RunCommandSettings
    ShellCommand.cs              settings nested as ShellCommand.ShellCommandSettings
  Services/                     pure, unit-testable logic used by the commands above
    Templates.cs                 init/new file contents
    CompilationPipeline.cs       ICompilationPipeline: lex → parse → check, concatenated errors
    EntryPoint.cs                run-entry rule (entry module needs top-level statements)
    BuildCache.cs                content-hash incremental build state (.pirate/cache.json)
    DiagnosticRenderer.cs        file:line:col + source excerpt + caret rendering
    ErrorMapper.cs               the only place *ErrorKind enums become LEX/SYN/SEM code strings
    TypeRegistrar.cs             Spectre ITypeRegistrar over Microsoft.Extensions.DependencyInjection
    TypeAdapter.cs               ITypeResolver over the built IServiceProvider

Pirate.Shared.File/              separate project (see docs/architecture/v2-architecture.md)
  FileDiscovery.cs               generic "find *.<ext> in a directory"
  FileNameResolver.cs            generic "argument -> resolved base name"
  PirateFileLocator.cs           *.pirate file discovery, recursive (wraps FileDiscovery)
  PirateFileName.cs              filename argument -> resolved module name, default "main" (wraps FileNameResolver)
  FleetFileLocator.cs            *.fleet discovery, non-recursive (wraps FileDiscovery)
  FleetFileName.cs               -n|--name argument -> resolved base name, default "module" (wraps FileNameResolver)

Pirate.Fleet/                    separate project (see docs/FLEET.md)
  FleetFile.cs                   the ".fleet" manifest model
  FleetFileRepository.cs         reads/writes "<name>.fleet" (via Pirate.Shared.File's FleetFileLocator)
  FleetEntryPoint.cs             argument -> .fleet entryPoint -> "main" resolution
```

Every command's settings class is nested inside its command
(`BuildCommand.BuildCommandSettings`, not a sibling top-level class) so the
type name always reads as "the settings for this specific command."

`PirateFileLocator`/`PirateFileName`/`FleetFileLocator`/`FleetFileName`
live together in `Pirate.Shared.File`, rather than `Pirate.Cli/Services/`
— they're general file-path logic with no dependency on the CLI framework
(or, for the `Fleet*` pair, on `Pirate.Fleet`'s manifest-specific
model/read-write logic), unlike `Templates` (init/new file contents) which
stays in `Pirate.Cli/Services` since it's CLI-specific. `NewCommand`'s
valid-type list (`NewCommand.NewFileTypes`) isn't in `Templates` either —
see "`pirate new [type] [filename]`" above. `Services/` holds that pure
logic specifically so it's unit-testable without touching the
filesystem-and-console-heavy `Execute` methods — see
[`docs/TESTING.md`](TESTING.md) for the general policy. `Pirate.Cli.Test`
covers `Templates`, `Banner`, the `CompilationPipeline`, and `EntryPoint`;
`Pirate.Shared.File.Test` covers all four
of `Pirate.Shared.File`'s file-path classes. `Commands/*.Execute` methods are thin
(argument resolution + one or two I/O calls + console output) and are
exercised by manual/e2e testing rather than unit tests, per the same
policy — but a settings class's `Validate()` is a decision, not plumbing,
so it gets its own unit tests too (`NewCommandSettingsTests`). Once
`build`/`run` actually compile and execute code, `Pirate.Spec.Test`
scenarios become the primary coverage for those `Execute` code paths.
