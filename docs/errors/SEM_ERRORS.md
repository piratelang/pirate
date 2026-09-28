# Semantic Errors (SEM)

Errors in this category mean the syntax is valid Pirate, but types, names,
or structural constraints don't check out. These are caught at compile time,
not at runtime.

Implemented by `Pirate.Semantics`; codes are assigned at presentation
(`Pirate.Cli`'s `ErrorMapper`) from `SemanticsErrorKind` values.

---

### SEM-001 — Undeclared variable

```
main.pirate:3:5 Undeclared variable 'x' *SEM-001*
  2 | func main() : void {
  3 |     x = 5;
    |     ^
```

**Cause**: A name is read or assigned without being declared in any visible
scope. (Assignment targets report this too — only `const` targets report
SEM-004.)
**Fix**: Declare the variable first: `var x = 5;`

### SEM-002 — Undeclared function

```
main.pirate:2:5 Undeclared function 'Printx' *SEM-002*
  1 | import standard Terminal;
  2 | Printx("hello");
    | ^^^^^^
```

**Cause**: A function is called that hasn't been declared, imported via
`import standard`, or brought in by `extern`.
**Fix**: Check the spelling, or add an `import standard <Namespace>;` /
`extern <dotted.path>;` declaration.

### SEM-003 — Type mismatch

```
main.pirate:2:13 Type mismatch: expected 'int', got 'string' *SEM-003*
  2 |     int x = "hello";
    |             ^
```

**Cause**: An expression's type doesn't match the declared type.
**Fix**: Either change the declared type or the expression's value.

### SEM-004 — Cannot assign to const

```
main.pirate:3:5 Cannot assign to 'limit' (declared 'const') *SEM-004*
  2 |     const int limit = 10;
  3 |     limit = 20;
    |     ^^^^^
```

**Cause**: An assignment — plain or through an index (`list[i] = ...` on a
`const` list) — targets a variable declared with `const`.
**Fix**: Declare it mutably instead: `var limit = 10;` or `int limit = 10;`

### SEM-005 — Missing return in non-void function

```
main.pirate:1:1 Function 'add' has no reachable 'return' *SEM-005*
  1 | func add(int a, int b) : int { }
    | ^
```

**Cause**: A function with a non-`void` return type doesn't have a `return`
statement.
**Fix**: Add a `return` with a value of the correct type.

### SEM-006 — Void function returns value

```
main.pirate:2:5 void function cannot return a value *SEM-006*
  1 | func main() : void {
  2 |     return 42;
    |     ^
```

**Cause**: A `void` function has a `return` with a value.
**Fix**: Either remove the value (`return;`) or change the return type.

### SEM-007 — Return value required

```
main.pirate:2:5 return value required in non-void function *SEM-007*
  1 | func main() : int {
  2 |     return;
    |     ^
```

**Cause**: A non-`void` function has a `return` without a value.
**Fix**: Add a value: `return 0;`

### SEM-008 — Empty array requires element type

```
main.pirate:2:5 var cannot be initialized with empty array *SEM-008*
  2 |     var list = [];
    |     ^
```

**Cause**: An empty array literal `[]` has no element type to infer.
**Fix**: Use an explicit type: `int[] list = [];`

### SEM-009 — for-in iterable must be an array

```
main.pirate:2:14 for-in iterable must be an array, got 'int' *SEM-009*
  2 |     for (item in count) { }
    |              ^
```

**Cause**: A for-in loop iterates a non-array expression.
**Fix**: Iterate an array, or use a counting for loop: `for var i = 0 to n`

### SEM-010 — Duplicate declaration

```
main.pirate:3:5 Duplicate declaration 'x' in scope *SEM-010*
  2 |     var x = 5;
  3 |     var x = 10;
    |     ^
```

**Cause**: A variable is declared twice in the same scope.
**Fix**: Use a different name, or remove the duplicate.

### SEM-011 — Unknown extern

```
main.pirate:1:1 Unknown extern 'Standard.Foo.Bar' *SEM-011*
  1 | extern Standard.Foo.Bar;
    | ^^^^^^^^^^^^^^^^^^^^^^^^
```

**Cause**: An `extern` declares a dotted path that is not in the builtin
registry, so no native implementation could ever back the call.
**Fix**: Check the spelling against the standard library's dotted names.

### SEM-012 — Unknown import

```
main.pirate:1:1 Unknown import 'standard IO' *SEM-012*
  1 | import standard IO;
    | ^^^^^^^^^^^^^^^^^^
```

**Cause**: `import standard <name>` names a builtin namespace group that has
no members — `Standard.<name>` matches no registered builtins.
**Fix**: Use an existing group, e.g. `import standard Terminal;` or
`import standard String;`.

### SEM-013 — Module import not supported yet

```
main.pirate:1:1 Import of module 'data' is not supported yet — the module
linker is a future milestone *SEM-013*
  1 | import module data as Data;
    | ^^^^^^^^^^^^^^^^^^^^^^^^^^
```

```
main.pirate:1:1 Import of external 'shared' is not supported yet — the module
linker is a future milestone *SEM-013*
  1 | import external shared as Shared;
    | ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
```

**Cause**: `import module` / `import external` syntax is defined by the
grammar but the cross-module linker has not landed. The message names
whichever kind of import was rejected.
**Fix**: Not user-fixable yet — the code is valid grammar pending the
module-linking milestone.

### SEM-014 — Return at top level

```
main.pirate:2:1 'return' is only valid inside a function *SEM-014*
  1 | var x = 5;
  2 | return;
    | ^^^^^^
```

**Cause**: A `return` appears among a module's top-level statements.
**Fix**: Put returns inside function bodies; top-level code simply ends.
