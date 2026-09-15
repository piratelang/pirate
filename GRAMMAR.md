# Grammar

This is a formal reference for the syntax currently implemented by the Lexer (`Pirate.Lexer.F`) and
Parser (`Pirate.Parser`). It intentionally only documents constructs that are actually parsed today.
See [SYNTAX.md](SYNTAX.md) for the same language with runnable examples.

A program is a sequence of top-level statements, executed in order. There is no required entry-point
function — a file can contain `extern` declarations, function definitions, variable declarations,
control statements and function calls directly at the top level.

```
Program:
    <Statement>*

Statement:
    <Extern Declaration>
    <Function Definition>
    <Control Statement>
    <Assignment>
    <Comment>
    <Operation>
```

Every statement is terminated by a `;`, except a block statement (`if`, `else`, `while`, `for`,
`func`), which is terminated by its closing `}`.

## Extern Declaration

Registers a function from the standard library under its last name segment, so it can be called
directly.

```
extern <dotted identifier>;
```

Example: `extern Standard.Terminal.Print;` registers `Standard.Terminal.Print` as `Print(...)`.

## Definition

### Function Definition

```
func <identifier>(<parameter>, ...): <type>
{
    <Statement>*
    [return <expression>;]
}
```

A `return` statement, if present, must be the last thing in the function body.

### Parameter

```
<type> <identifier>
```

## Control Statement

### If Statement

```
if <Expression.Comparison>
{
    <then>
}
```

### Else Statement

```
else
{

}
```

`elif`/`else if` is not implemented — only a single trailing `else` is supported.

### While Loop

```
while <Expression.Comparison>
{

}
```

### For Loop

```
for <Assignment.Declaration.int> to <Value.int>
{

}
```

The lexer recognizes a `foreach`/`in` keyword pair, but no parser handles them yet, so
`foreach (... in ...)` is not usable.

## Assignment

### Declaration

```
<type> <identifier> = <expression>
var <identifier> = <expression>
```

`<type>` is one of `int`, `float`, `string`, `char`.

### Reassignment

```
<identifier> = <expression>
```

The variable must already be declared.

## Comment

```
// <anything> ;
```

Everything between `//` and the next `;` is discarded. The trailing `;` is required — there is no
end-of-line comment form.

## Operation

### Comparison

```
<value> <comparisonoperator> <value>
```

Operators:

```
==, !=, <, <=, >, >=
```

There is no logical AND/OR (`&&`/`||`/`and`/`or`) — those tokens are not produced by the lexer, and
the parser has no keyword or operator for them.

### Binary

```
<value> <operator> <value>
```

Operators:

```
+, -, *, /, ^, %
```

Binary operations are left-associative: `1 + 2 + 3` parses as `(1 + 2) + 3`, i.e. the root node's
left child is `1 + 2` and its right child is `3`.

## Value

```
String:
    "<String>"
Float:
    <int>.<int>
Char:
    '<Char>'
Int:
    <Int>
Variable Identifier:
    <identifier>
Function Call:
    <function name>(<expression>, ...)
```

## Not yet implemented

These keywords/tokens are recognized by the lexer or referenced elsewhere in the codebase, but have
no parser support, so programs using them will fail to parse:

- `foreach` / `in` (loop over a collection)
- `class` / `new` (classes/instantiation)
- Boolean literals (`true`/`false`) — a `BooleanValue` exists internally as the result of a
  comparison, but there is no literal syntax for it
- List/array literals (`[...]`) and indexing (`list[index]`)
- `elif`/`else if`
