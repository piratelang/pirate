# Error Code System — Internal Reference

How the Pirate error system works in code, and how to add new errors.

## Architecture

```
Lexer.cs                  Parser.fs                 Semantics (future)
  │                         │                          │
  ▼                         ▼                          ▼
LexError                SyntaxError               SemanticsError
  ├─ Kind                  ├─ Kind                   ├─ Kind
  ├─ Message               ├─ Message                ├─ Message
  └─ Locations             └─ Locations              └─ Locations
  │                         │                          │
  └─────────────────────────┼──────────────────────────┘
                            ▼
                    Pirate.Cli.Services.ErrorMapper
                            │
                            ▼
                    Code string: "SYN-005"
                            │
                            ▼
                    Pirate.Cli.Services.DiagnosticRenderer
                            │
                            ▼
                    main.pirate:4:12 ... *SYN-005*
                      3 | ...
                      4 |     var x = 5
                        |              ^
```

## Adding a New Error

1. **Add the enum value** to the appropriate `*ErrorKind` enum in
   `Pirate.Syntax/*.cs`:
   ```csharp
   // Pirate.Syntax/SyntaxError.cs
   public enum SyntaxErrorKind {
       // ... existing values ...
       MissingSemicolonAfterForLoop, // new
   }
   ```

2. **Add the switch arm** in `ErrorMapper.cs`:
   ```csharp
   // Pirate.Cli/Services/ErrorMapper.cs
   private static string MapSyntax(SyntaxErrorKind kind) => kind switch {
       // ... existing mappings ...
       SyntaxErrorKind.MissingSemicolonAfterForLoop => "SYN-044",
       _ => "SYN-999",
   };
   ```

3. **Emit the error** in the lexer/parser/semantics:
   ```csharp
   // Lexer.cs or Parser.fs
   _errors.Add(new SyntaxError(
       SyntaxErrorKind.MissingSemicolonAfterForLoop,
       "Expected ';' after for loop",
       startLocation));
   ```

4. **Document it** in the appropriate `docs/external/*_ERRORS.md` file:
   ```markdown
   ### SYN-044 — Missing ';' after for loop
   ...explanation, example, fix...
   ```

## Naming Convention for Enum Values

- **Short and concise**: `MissingSemicolonAfterDeclaration`, not
  `MissingSemicolonAfterVariableDeclarationStatement`
- **Category tells you the context**: The enum type (`SyntaxErrorKind`)
  already says "syntax", so no need to repeat it
- **Action + target**: `MissingX`, `ExpectedY`, `UnexpectedZ`

## Numbering Convention

Gaps in numbering allow inserting new errors without renumbering:

| Range | Category | Values |
|-------|----------|--------|
| `SYN-001`–`SYN-014` | Missing tokens | 14 slots, ~8 used |
| `SYN-020`–`SYN-027` | Missing identifiers | 8 slots |
| `SYN-030`–`SYN-034` | Missing keywords | 5 slots |
| `SYN-040`–`SYN-043` | Unexpected input | 4 slots, ~4 used |

## Error Type Hierarchy

```
Pirate.Syntax/
  CompilationError.cs   ← abstract base class
  LexError.cs           ← LexErrorKind (7 values)
  SyntaxError.cs        ← SyntaxErrorKind (28 values)
  SemanticsError.cs     ← SemanticsErrorKind (10 values)
```

Key design decisions:
- `EndLocation` is **nullable** — `null` means point error, populated means
  range error (cleaner semantics, renderer handles both)
- `CompilationError` is a **class** (not a record struct) — we collect
  many errors, and struct copying on every `List.Add` would be wasteful
- Error codes are **CLI-only** — the compiler libraries are code-agnostic
