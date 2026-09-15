# The `.fleet` project manifest

Status: **canonical** for the `.fleet` file format (`Pirate.Fleet`,
`src-v2/Pirate.Fleet`). See
[`docs/architecture/v2-architecture.md`](architecture/v2-architecture.md)
for where `Pirate.Fleet` sits in the project graph, and
[`docs/CLI.md`](CLI.md) for how `init`/`run` use it.

## What it is

`.fleet` is a `package.json`-style manifest, one per project, identifying a
directory as a Pirate project and declaring its metadata. It is JSON, and
lives in the project root as `<name>.fleet` — `<name>` defaults to `module`
(so `pirate init` with no options writes `module.fleet`), overridable via
`pirate init -n|--name <name>`, same shape as `[filename]` defaulting to
`main` for the `.pirate` file it writes alongside it.

```json
{
  "name": "my-project",
  "version": "0.1.0",
  "entryPoint": "main",
  "build": {},
  "dependencies": {}
}
```

| Field | Meaning | Default (`pirate init`) |
|---|---|---|
| `name` | Project name. | The project directory's folder name. |
| `version` | Project version (free-form string, not yet validated as semver). | `"0.1.0"` |
| `entryPoint` | The module `pirate run` executes when invoked with no `[filename]` argument. | Whatever `pirate init`'s own `[filename]` argument resolved to (`"main"` by default). |
| `build` | Reserved for future compiler/build configuration. | `{}` |
| `dependencies` | Reserved for a future package ecosystem. | `{}` |

**`build` and `dependencies` are not implemented yet** — same
honesty-about-pipeline-status policy as `pirate build`/`run` themselves
(see `docs/architecture/v2-architecture.md`, "Current state"). They exist
in the schema as empty JSON objects so a `.fleet` written today round-trips
cleanly once real fields land there, but nothing in `Pirate.Cli` or the
compiler reads them yet. There's nothing to configure a build with (no
compiler) and nothing to depend on (v2's only "import" mechanism is
`extern Standard.X.Y;`, resolved entirely against the built-in standard
library — see `docs/architecture/v2-architecture.md`'s `Pirate.StandardLibrary`
section).

`.fleet` deliberately does **not** list project files — `pirate build`'s
directory-scan discovery (`Pirate.Shared.File`'s `PirateFileLocator`) is
unaffected and still finds every `*.pirate` file recursively.

## Finding the manifest

Since the manifest's own base name is chosen at `init` time (not fixed),
anything that needs to find it later — `pirate run`'s entry-point
resolution — scans the directory instead of checking a known name:
`Pirate.Shared.File.FleetFileLocator.DiscoverFleetFiles(directory)` looks
for `*.fleet` directly inside `directory` (not recursively — a manifest
identifies a project root, unlike a `.pirate` module which can live
anywhere in the tree). It lives in `Pirate.Shared.File` alongside
`PirateFileLocator`, not in `Pirate.Fleet` — see
[`docs/architecture/v2-architecture.md`](architecture/v2-architecture.md)'s
`Pirate.Shared.File`/`Pirate.Fleet` sections for why. A project is expected
to have exactly one `.fleet`; if more than one exists, the first
alphabetically is used, deterministically but arbitrarily — this is an
edge case, not a supported multi-manifest feature.

## Entry-point resolution

`pirate run [filename]` (and `pirate init [filename]`, which writes
`entryPoint` to match) resolve the module name in this order
(`Pirate.Fleet`'s `FleetEntryPoint.Resolve`):

1. An explicit `[filename]` argument — always wins.
2. The `entryPoint` of the `*.fleet` found in the current directory, if any.
3. `"main"` (`Pirate.Shared.File`'s `PirateFileName` default), if neither
   of the above apply.

A project with no `.fleet` behaves exactly as before `.fleet` existed —
this is additive, not a breaking change for existing `.pirate` directories.

`pirate build`'s no-argument behavior (recursive discovery of every
`.pirate` file) does **not** consult `.fleet` — that's a fundamentally
different operation ("find everything") from `run`'s ("resolve one file"),
and there's currently no "build resolves a single default file" path for
an entry point to slot into. `build [filename]`'s explicit-filename path
is unaffected either way, since an explicit argument already takes
precedence.

## Reading/writing a project's `.fleet`

`Pirate.Fleet.FleetFileRepository`:

- `Exists(directory)` — whether any `*.fleet` is present (via
  `FleetFileLocator`).
- `TryRead(directory)` — the parsed `FleetFile` from whichever `*.fleet` is
  found, or `null` if none exists. Throws `System.Text.Json.JsonException`
  (with the offending file's name in the message) if one exists but isn't
  valid JSON; callers (e.g. `RunCommand`) catch this and print a clear
  "...could not be read..." error rather than a raw stack trace or a
  silent fallback — a corrupt manifest is a foreseeable, actionable state.
- `Write(directory, name, fleet)` — writes a pretty-printed
  `<name>.fleet`. `Pirate.Shared.File.FleetFileName.Resolve(argument)`
  turns a user-supplied `-n|--name` value (or `null`) into that `name` —
  same default/extension-stripping shape as `PirateFileName.Resolve`
  (both wrap `Pirate.Shared.File`'s generic `FileNameResolver`), just for
  `.fleet`/`module` instead of `.pirate`/`main`.

`pirate init` refuses to write a `.fleet` if one already exists under any
name (prints an error, exits `1`) rather than silently clobbering a
project's existing metadata — the same overwrite-protection policy
`pirate new`'s `gitignore`/`gitattributes`/`pirate` cases already follow
(see `docs/CLI.md`). Note this guard currently applies only to the `.fleet`
write; `pirate init`'s pre-existing `.pirate` file write has no such check
and always overwrites — a separate, longer-standing gap this feature
didn't introduce and doesn't fix.
