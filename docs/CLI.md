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

Every command's settings class inherits `GlobalSettings` (an intentionally
empty `CommandSettings` subclass, for now) — the seam any future flag or
behavior meant to apply to every command hangs off, rather than adding it to
`RunCommandSettings`/`InitCommandSettings`/etc. individually. Nothing uses
it today.

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
without it, matching v1). Unlike v1's template (untyped `func main() { print(...); }`,
which predates static typing), the v2 template is grammar-valid under
[`docs/GRAMMAR.md`](GRAMMAR.md):

```pirate
extern Standard.Terminal.Print;

func main() : void
{
    Print("Hello World");
}
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
directory recursively (same as v1's
`Directory.GetFiles("./", "*.pirate", SearchOption.AllDirectories)`); with
one, resolves and builds just that file (same resolution as `run`). Either
way the discovered file(s) are listed in a table. **Discovery/resolution is
real; compiling them is not.** v1's module-list-based incremental rebuild
(`Shell.ModuleList`/`ModuleListRepository`) has no v2 equivalent yet — there
is nothing to incrementally rebuild until the lexer/parser/semantics/compiler
pipeline exists (see `docs/architecture/v2-architecture.md`, "Current
state"). `build` prints a clear stub message rather than silently pretending
to compile.

Discovery runs inside an `AnsiConsole.Progress()` block, one `ProgressTask`
per file — today that task completes the instant the file is confirmed
(there's nothing to compile yet), but the per-file granularity is already
there for when `build` actually compiles each module. The results render
with the same look as the no-args banner's command list and `new`'s
options table — a `Rule` divider over a borderless, bold-row `Table` —
rather than the bordered ASCII grid this used to be.

Examples:

```
$ pirate build
── Discovered modules ──────────────────────────────────────────
.example\main.pirate
.example\test.pirate
main.pirate
Compilation is not implemented yet — the v2 lexer/parser/semantics/compiler
pipeline is still a stub (see docs/architecture/v2-architecture.md).

$ pirate build main
── Discovered modules ──────────────────────────────────────────
main.pirate
Compilation is not implemented yet — ...

$ pirate build nope
File "nope.pirate" not provided or does not exist.
```

### `pirate run [filename]`

Resolves `filename` and checks the `.pirate` file exists. **File resolution
is real; execution is not** — same pipeline-not-built-yet reason as `build`.
Once the VM exists, `run` will lex, parse, check, compile, and execute the
file and stream stdout, matching v1's `RunCommand` behavior of building then
interpreting.

With an explicit `filename`, resolution is unchanged from v1 — accepted
with or without the `.pirate` extension. With **no** `filename`, resolution
now goes through [`.fleet`](FLEET.md) (`Pirate.Fleet`'s `FleetEntryPoint`):
whatever `*.fleet` file `FleetFileLocator` finds in the current directory
(any name — `pirate init -n` can call it anything) supplies its
`entryPoint` if one exists, falling back to `main` otherwise — matching
v1's plain "`main` is the default" behavior for any directory without a
`.fleet`. A `.fleet` that exists but isn't valid JSON is reported clearly
and exits `1`, rather than silently falling back or crashing with a raw
stack trace.

The resolve-and-check step runs inside an `AnsiConsole.Status()` spinner —
today that's instant, but it's the seam where lex/parse/check/compile/execute
will hang once the pipeline exists, so `run` won't need restructuring to show
progress on real work later.

Examples:

```
$ pirate run main
Resolving main.pirate...
Found main.pirate, but execution is not implemented yet — the v2
lexer/parser/semantics/compiler/VM pipeline is still a stub (see
docs/architecture/v2-architecture.md).

$ pirate run nope
Resolving nope.pirate...
File "nope.pirate" not provided or does not exist.

$ pirate run
Resolving main.pirate...
Found main.pirate, but execution is not implemented yet — ...
```

The last example is with no `.fleet` present, so it falls back to `main` —
identical to `pirate run main`. With a `.fleet` whose `entryPoint` is
`"other"`, that same no-argument `pirate run` resolves `other.pirate`
instead; `pirate run main` still resolves `main.pirate` regardless, since
an explicit argument always wins. And with a `.fleet` that exists but isn't
valid JSON:

```
$ pirate run
".fleet" exists but could not be read: 'n' is an invalid start of a property
name. Expected a '"'. Path: $ | LineNumber: 0 | BytePositionInLine: 2.
```

### `pirate shell`

Opens a read-eval-print loop: prints the version banner, then reads lines
from stdin until `stop`, `exit`, or `break` (matching v1's exit terms) or
EOF. Uses plain `Console.ReadLine()`, not a Spectre `TextPrompt` — a
`TextPrompt` requires an interactive terminal and fails on piped/redirected
stdin, which would break both scripted usage and any future e2e test that
feeds a script into `pirate shell` via stdin. **Reading input is real;
lexing/evaluating each line is not** yet, same reason as `build`/`run`.

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
  appended by the SDK's deterministic-build feature. **Known wart:** the
  reflection lookup itself is currently duplicated as a private `Version()`
  method in both `Banner.cs` and `ShellCommand.cs`, rather than shared
  through one helper (a `CliInfo`-style type previously centralized this;
  it was removed) — the source of truth (the csproj property) is still
  single, but the lookup code isn't.
- `build`/`run`/`shell` are honest about pipeline status: they do the real,
  implementable-today part (file discovery/resolution/REPL loop) and print
  an explicit "not implemented yet" message for the part that depends on the
  lexer/parser/semantics/compiler/VM, rather than a silent no-op or a fake
  success.
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
  GlobalSettings.cs             base settings type every command's settings inherits (currently empty)
  Theme.cs                      shared markup colors (error/warning/success/info/accent)
  Commands/
    InitCommand.cs               settings nested as InitCommand.InitCommandSettings
    NewCommand.cs                settings nested as NewCommand.NewCommandSettings
    BuildCommand.cs              settings nested as BuildCommand.BuildCommandSettings
    RunCommand.cs                settings nested as RunCommand.RunCommandSettings
    ShellCommand.cs              settings nested as ShellCommand.ShellCommandSettings
  Services/                     pure, unit-testable logic used by the commands above
    Templates.cs                 init/new file contents

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
covers `Templates` and `Banner`; `Pirate.Shared.File.Test` covers all four
of `Pirate.Shared.File`'s file-path classes. `Commands/*.Execute` methods are thin
(argument resolution + one or two I/O calls + console output) and are
exercised by manual/e2e testing rather than unit tests, per the same
policy — but a settings class's `Validate()` is a decision, not plumbing,
so it gets its own unit tests too (`NewCommandSettingsTests`). Once
`build`/`run` actually compile and execute code, `Pirate.Spec.Test`
scenarios become the primary coverage for those `Execute` code paths.
