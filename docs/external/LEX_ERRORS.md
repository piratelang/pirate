# Lexical Errors (LEX)

Errors in this category mean the lexer couldn't split the source text into
valid tokens.

---

### LEX-001 — Unexpected character

```
main.pirate:1:1 Unexpected character '@' *LEX-001*
  1 | @foo
    | ^
```

**Cause**: A character that isn't valid Pirate syntax was found.
**Fix**: Remove or replace the character. Valid characters are letters,
digits, operators (`+` `-` `*` `/` `%` `^` `=` `!` `<` `>` `&` `|`),
brackets (`(` `)` `{` `}` `[` `]`), and punctuation (`,` `:` `;` `.` `'` `"`).

---

### LEX-002 — Unterminated string literal

```
main.pirate:3:5 Unterminated string literal *LEX-002*
  2 | func main() : void {
  3 |     var msg = "hello
    |               ^^^^^^
  4 | }
```

**Cause**: A double-quoted string was started with `"` but never closed.
**Fix**: Add the closing `"` at the end of the string.

---

### LEX-003 — Unterminated char literal

```
main.pirate:1:1 Unterminated char literal *LEX-003*
  1 | 'x
    | ^^
```

**Cause**: A single-quoted char literal was started with `'` but never closed.
**Fix**: Add the closing `'` — char literals must contain exactly one
character: `'x'`, `'\n'`, etc.

---

### LEX-004 — Unknown escape sequence

```
main.pirate:1:3 Unknown escape sequence '\q' *LEX-004*
  1 | "\qfoo"
    |   ^^
```

**Cause**: A backslash escape in a string or char literal isn't recognized.
**Fix**: Use one of the valid escapes: `\n` (newline), `\t` (tab),
`\"` (double quote), `\'` (single quote), `\\` (backslash).

---

### LEX-005 — Integer literal out of range

```
main.pirate:1:1 Integer literal '99999999999999999999' is out of range *LEX-005*
  1 | 99999999999999999999
    | ^^^^^^^^^^^^^^^^^^^^
```

**Cause**: An integer literal exceeds the range of a 32-bit signed integer
(±2,147,483,647).
**Fix**: Use a smaller value, or write it as a float literal if you need
a larger number: `99999999999.0`.

---

### LEX-006 — Lone ampersand (did you mean `&&`?)

```
main.pirate:1:3 Unexpected character '&' (did you mean '&&'?) *LEX-006*
  1 | a & b
    |   ^
```

**Cause**: A single `&` was found. Pirate uses `&&` for logical AND.
**Fix**: Change `&` to `&&`.

---

### LEX-007 — Lone pipe (did you mean `||`?)

```
main.pirate:1:3 Unexpected character '|' (did you mean '||'?) *LEX-007*
  1 | a | b
    |   ^
```

**Cause**: A single `|` was found. Pirate uses `||` for logical OR.
**Fix**: Change `|` to `||`.
