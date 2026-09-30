# Syntax Errors (SYN)

Errors in this category mean the token sequence doesn't form valid Pirate
syntax. The parser collects all syntax errors it can find rather than
stopping at the first one.

---

## Missing Tokens (SYN-001 — SYN-014)

### SYN-001 — Missing `{`

```
main.pirate:1:17 Expected '{' to begin block *SYN-001*
  1 | func main() : void
    |                 ^
```

**Fix**: Add `{` after the return type: `func main() : void {`

### SYN-002 — Missing `}`

```
main.pirate:3:1 Expected '}' to close block *SYN-002*
  2 | func main() : void {
  3 |
```

**Fix**: Add `}` to close the block.

### SYN-003 — Missing `;` after return

```
main.pirate:3:12 Expected ';' after return *SYN-003*
  2 | func main() : int {
  3 |     return 42
    |            ^
  4 | }
```

**Fix**: Add `;`: `return 42;`

### SYN-004 — Missing `;` after variable declaration

```
main.pirate:3:13 Expected ';' after declaration *SYN-004*
  3 |     var x = 5
    |             ^
```

**Fix**: Add `;`: `var x = 5;`

### SYN-005 — Missing `;` after expression

```
main.pirate:4:12 Expected ';' after expression *SYN-005*
  3 | func main() : void {
  4 |     var x = 5
    |            ^
  5 | }
```

**Fix**: Add `;`: `var x = 5;`

### SYN-006 — Missing `)` in parenthesized expression

```
main.pirate:2:10 Expected ')' after expression *SYN-006*
  2 | (1 + 2
    |      ^
```

**Fix**: Add `)`: `(1 + 2)`

### SYN-007 — Missing `)` in function call

```
main.pirate:2:15 Expected ')' to close call arguments *SYN-007*
  2 |     Print("hello"
    |               ^
```

**Fix**: Add `)`: `Print("hello")`

### SYN-008 — Missing `]` in array literal

```
main.pirate:2:10 Expected ']' to close array literal *SYN-008*
  2 |     [1, 2
    |     ^
```

**Fix**: Add `]`: `[1, 2]`

### SYN-009 — Missing `]` in array index

```
main.pirate:2:10 Expected ']' to close index *SYN-009*
  2 |     arr[0
    |     ^
```

**Fix**: Add `]`: `arr[0]`

### SYN-010 — Missing `)` in for-in loop

```
main.pirate:2:15 Expected ')' in for-in *SYN-010*
  2 |     for (item in items
    |               ^
```

**Fix**: Add `)`: `for (item in items) { ... }`

### SYN-011 — Missing `]` in array type

```
main.pirate:1:10 Expected ']' after '[' in type *SYN-011*
  1 | func f(int[ x) : void { }
    |          ^
```

**Fix**: Add `]`: `int[]`

### SYN-012 — Missing `(` after function name

```
main.pirate:1:6 Expected '(' after function name *SYN-012*
  1 | func main : void { }
    |      ^
```

**Fix**: Add `(`: `func main()`

### SYN-013 — Missing `)` after parameters

```
main.pirate:1:10 Expected ')' after parameters *SYN-013*
  1 | func f(int a, int b : void { }
    |          ^
```

**Fix**: Add `)`: `func f(int a, int b)`

### SYN-014 — Missing `:` before return type

```
main.pirate:1:13 Expected ':' before return type *SYN-014*
  1 | func main() void { }
    |             ^
```

**Fix**: Add `:`: `func main() : void`

---

## Missing Identifiers (SYN-020 — SYN-028)

### SYN-020 — Expected identifier after `extern`

```
main.pirate:1:8 Expected identifier after 'extern' *SYN-020*
  1 | extern ;
    |        ^
```

**Fix**: Add the qualified function name: `extern Standard.Terminal.Print;`

### SYN-021 — Expected identifier after `.`

```
main.pirate:1:15 Expected identifier after '.' *SYN-021*
  1 | extern Standard.
    |               ^
```

**Fix**: Add the next name part: `extern Standard.Terminal.Print`

### SYN-022 — Missing function name

```
main.pirate:1:6 Expected function name *SYN-022*
  1 | func () : void { }
    |      ^
```

**Fix**: Add a name: `func main() : void { }`

### SYN-023 — Missing parameter name

```
main.pirate:1:10 Expected parameter name *SYN-023*
  1 | func f(int ) : void { }
    |          ^
```

**Fix**: Add a parameter name: `func f(int x) : void { }`

### SYN-024 — Expected identifier after `var`

```
main.pirate:2:5 Expected identifier after 'var' *SYN-024*
  2 |     var = 5;
    |     ^
```

**Fix**: Add a variable name: `var x = 5;`

### SYN-025 — Expected identifier after type

```
main.pirate:2:5 Expected identifier after type *SYN-025*
  2 |     int = 5;
    |     ^
```

**Fix**: Add a variable name: `int x = 5;`

### SYN-026 — Expected identifier in for-in

```
main.pirate:2:6 Expected identifier in for-in *SYN-026*
  2 |     for ( in items) { }
    |      ^
```

**Fix**: Add a loop variable: `for (item in items) { }`

### SYN-027 — Expected identifier in for

```
main.pirate:2:9 Expected identifier in for *SYN-027*
  2 |     for var = 0 to 10 { }
    |         ^
```

**Fix**: Add a loop variable: `for var i = 0 to 10 { }`

---

### SYN-028 — Expected identifier after `const`

```
main.pirate:3:11 Expected identifier after 'const' *SYN-028*
  2 | func main() : void {
  3 |     const = 5;
    |           ^
```

**Cause**: `const` must be followed by a name, optionally preceded by a
type or `var`.
**Fix**: `const x = 5;`, `const int x = 5;`, or `const var x = 5;`


## Missing Keywords (SYN-030 — SYN-034)

### SYN-030 — Expected `var` or `(` after `for`

```
main.pirate:2:5 Expected 'var' or '(' after 'for' *SYN-030*
  2 |     for i = 0 to 10 { }
    |     ^
```

**Fix**: Add `var` for counting loop: `for var i = 0 to 10 { }`
Or use `(` for for-in: `for (item in items) { }`

### SYN-031 — Expected `in` in for-in

```
main.pirate:2:10 Expected 'in' in for-in *SYN-031*
  2 |     for (item items) { }
    |          ^
```

**Fix**: Add `in`: `for (item in items) { }`

### SYN-032 — Expected `=` in variable declaration

```
main.pirate:2:9 Expected '=' in variable declaration *SYN-032*
  2 |     var x 5;
    |         ^
```

**Fix**: Add `=`: `var x = 5;`

### SYN-033 — Expected `=` in for loop

```
main.pirate:2:11 Expected '=' in for *SYN-033*
  2 |     for var i 0 to 10 { }
    |           ^
```

**Fix**: Add `=`: `for var i = 0 to 10 { }`

### SYN-034 — Expected `to` in for loop

```
main.pirate:2:17 Expected 'to' in for *SYN-034*
  2 |     for var i = 0 10 { }
    |                 ^
```

**Fix**: Add `to`: `for var i = 0 to 10 { }`

---

## Unexpected Input (SYN-041 — SYN-049)

### SYN-041 — Expected expression

```
main.pirate:2:10 Expected expression, got '}' *SYN-041*
  2 |     var x = ;
    |          ^
```

**Fix**: Add an expression after `=`: `var x = 5;`

### SYN-042 — Unexpected end of input in expression

```
main.pirate:2:9 Unexpected end of input in expression *SYN-042*
  2 |     var x = 1 +
    |         ^
```

**Fix**: Complete the expression: `var x = 1 + 2;`

### SYN-043 — Expected type

```
main.pirate:1:14 Expected type, got 'void' *SYN-043*
  1 | func f(void x) : void { }
    |              ^
```

**Fix**: `void` is not a valid parameter type. Use `int`, `float`,
`string`, `char`, or `bool`.

### SYN-044 — Expected import source after `import`

```
main.pirate:1:8 Expected 'standard', 'module', or 'external' after 'import' *SYN-044*
  1 | import Terminal;
    |        ^^^^^^^^
```

**Cause**: `import` must be followed by one of the soft keywords
`standard`, `module`, or `external`.
**Fix**: `import standard Terminal;`

### SYN-045 — Expected a name after `import standard|module|external`

```
main.pirate:1:16 Expected a name after 'import' *SYN-045*
  1 | import standard ;
    |                ^
```

**Fix**: name what you're importing: `import standard Terminal;`

### SYN-046 — Expected identifier after `as`

```
main.pirate:1:29 Expected identifier after 'as' *SYN-046*
  1 | import module data as ;
    |                             ^
```

**Fix**: give the import an alias: `import module data as Data;`

### SYN-047 — Missing `;` after import

```
main.pirate:1:1 import standard Terminal *SYN-047*
  1 | import standard Terminal
    |                             ^
```

**Fix**: terminate the statement: `import standard Terminal;`

### SYN-048 — Expected declaration after `private`

```
main.pirate:1:9 Expected 'func', 'var', 'const', or a type after 'private' *SYN-048*
  1 | private if x { }
    |        ^
```

**Cause**: `private` only prefixes function and variable declarations.
**Fix**: `private func get() : int { ... }` or `private var data = ...;`

### SYN-049 — `as` alias on a standard import

```
main.pirate:1:26 'as' only applies to module and external imports *SYN-049*
  1 | import standard Terminal as T;
    |                          ^^
```

**Cause**: `import standard <NS>;` binds the group's own names (leaf and
dotted path) — there is nothing for an alias to name.
**Fix**: drop the alias: `import standard Terminal;`

## Class-file shapes (SYN-050 — SYN-059)

Class files (`.cpirate`/`.cpir`, GRAMMAR.md §4) are parsed with a
different top-level rule than modules — no loose statements, only
fields, constants, constructors, and methods.

### SYN-050 — Expected a class member

```
Counter.cpirate:1:1 Expected 'field', 'const', 'constructor', or a method in a class file *SYN-050*
  1 | var x = 5;
    | ^^^
```

**Cause**: A class file has no loose statements — every top-level line
must be a field, constant, constructor, or method.
**Fix**: Use `field int x = 5;` for a field, or move the statement
inside a constructor or method body.

### SYN-051 — Missing `(` after `constructor`

```
Counter.cpirate:1:12 Expected '(' after 'constructor' *SYN-051*
  1 | constructor int x) { }
    |            ^
```

**Fix**: `constructor(int x) { }`

### SYN-052 — Missing `)` after constructor parameters

```
Counter.cpirate:1:16 Expected ')' after constructor parameters *SYN-052*
  1 | constructor(int x { }
    |                   ^
```

**Fix**: close the parameter list: `constructor(int x) { }`

### SYN-053 — Expected `self` after `:` in a constructor

```
Counter.cpirate:1:16 Expected 'self' after ':' in constructor *SYN-053*
  1 | constructor() : foo() { }
    |                ^^^
```

**Cause**: A constructor delegate can only call another constructor on
the same class, spelled `self`.
**Fix**: `constructor() : self(16) { }`

### SYN-054 — Missing `(` after `self`

```
Counter.cpirate:1:22 Expected '(' after 'self' *SYN-054*
  1 | constructor() : self { }
    |                      ^
```

**Fix**: `constructor() : self(16) { }` — `self` as a delegate always
calls another constructor, so it always takes an argument list.

### SYN-055 — Missing `)` after delegate arguments

```
Counter.cpirate:1:27 Expected ')' to close delegate arguments *SYN-055*
  1 | constructor() : self(16 { }
    |                          ^
```

**Fix**: close the argument list: `constructor() : self(16) { }`

### SYN-056 — Expected a class name after `new`

```
main.pirate:1:22 Expected a class name after 'new' *SYN-056*
  1 | Counter c = new (10);
    |                      ^
```

**Fix**: name the class being created: `new Counter(10)`

### SYN-057 — Missing `(` after a `new` class name

```
main.pirate:1:27 Expected '(' after 'new' class name *SYN-057*
  1 | Counter c = new Counter 10);
    |                          ^
```

**Fix**: `new Counter(10)`

### SYN-058 — Missing `)` to close `new` arguments

```
main.pirate:1:31 Expected ')' to close 'new' arguments *SYN-058*
  1 | Counter c = new Counter(10;
    |                            ^
```

**Fix**: `new Counter(10)`

### SYN-059 — Reserved keyword, not supported yet

```
Counter.cpirate:1:1 'extends' is reserved, not supported yet *SYN-059*
  1 | extends Base;
    | ^^^^^^^
```

**Cause**: `extends`, `implements`, `abstract`, `override`, `static`,
and `super` (as a constructor delegate) are reserved for a later
milestone (docs/brainstorm/FLAT_PLAN.md, "After the first slice") —
the classes first slice is fields, constructors, methods, `self`, and
nullable types only.
**Fix**: Not user-fixable yet.
