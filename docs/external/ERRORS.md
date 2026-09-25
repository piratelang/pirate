# Pirate Error Codes

When the Pirate compiler encounters an error, it reports it with a **code**,
a **message**, and a **source excerpt** highlighting exactly where the
problem is:

```
main.pirate:4:12 Expected ';' after expression *SYN-005*
  3 | func main() : void {
  4 |     var x = 5
    |              ^
  5 | }
```

## Code Structure

Every error code has a 3-letter prefix identifying the pipeline stage and
a sequential number:

| Prefix | Stage | Meaning |
|--------|-------|---------|
| **LEX** | Lexer | The source text couldn't be split into valid tokens |
| **SYN** | Parser | The token sequence doesn't form valid syntax |
| **SEM** | Semantics | The syntax is valid, but types or names don't check out |
| **RTN** | Runtime | The program crashed while executing (future) |

## How to Read an Error

1. **File and position** — `main.pirate:4:12` tells you the file, line, and column
2. **Message** — describes what was expected vs. what was found
3. **Code** — `*SYN-005*` lets you look up the full explanation below
4. **Excerpt** — shows the surrounding source with a `^` (or `^^^^^^^` for
   ranges) pointing at the exact problem

## Error Catalogs

- **[Lexical Errors](LEX_ERRORS.md)** — `LEX-001` through `LEX-007`
- **[Syntax Errors](SYN_ERRORS.md)** — `SYN-001` through `SYN-043`
- **[Semantic Errors](SEM_ERRORS.md)** — `SEM-001` through `SEM-010` *(future)*
- **[Runtime Errors](RTN_ERRORS.md)** — `RTN-001` through `RTN-003` *(future)*

## Multiple Errors

The Pirate compiler doesn't stop at the first error. It collects **all**
errors it can find and prints them together, so you can fix multiple
problems in one edit-run cycle:

```
main.pirate:4:12 Expected ';' after expression *SYN-005*
main.pirate:7:5 Expected identifier after 'var' *SYN-024*
helper.pirate:2:1 Expected 'extern' or 'func', got 'bogus' *SYN-040*
```

## Verbose Mode

Run with `-v` or `--verbose` to see additional details on successful builds:

```
$ pirate build -v
  ⟳ main.pirate (rebuilt)
    hash: sha256:abc123...
  ✓ helper.pirate (up to date)
    hash: sha256:def456... (unchanged)
```
