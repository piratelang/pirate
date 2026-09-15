# Syntax

A tutorial-style walkthrough of the pirate syntax that is actually implemented today. For the formal
grammar, see [GRAMMAR.md](GRAMMAR.md) — this file and that one describe the same language and should
stay in sync.

## Structure

A pirate file is a sequence of statements, most commonly `extern` declarations and `func`
definitions. There is no required `func main()` entry point enforced by the interpreter — statements
at the top level (including function calls) run in the order they appear:

```nim
extern Standard.Terminal.Print;

Print("Hello World");
```

Most statements end with a `;`. Block statements (`if`, `else`, `while`, `for`, `func`) are instead
closed by their `}`.

### Comments

```nim
// This is a comment;
```

Everything between `//` and the next `;` is ignored. The trailing `;` is required — there is no
single-token or end-of-line comment form.

### Extern

Registers a function from the standard library, under its last name segment, so it can be called
directly:

```nim
extern Standard.Terminal.Print;

Print("Ahoy!");
```

## Variables

### Declaration

```nim
var name = "value";
string name = "value";
int count = 1;
float ratio = 1.5;
char letter = 'a';
```

`var` infers nothing special — it is simply a declaration without an explicit type. Declaring with an
explicit type (`int`, `float`, `string`, `char`) is also supported.

### Reassignment

Once declared, a variable can be reassigned without repeating the type or `var`:

```nim
count = 2;
```

### Values

```nim
"a string"
1.5
'c'
1
identifier
```

Strings support the `\n` and `\t` escape sequences. There is no boolean literal (`true`/`false`) —
the only way to produce a boolean value is as the result of a comparison.

## Operators

### Binary

```nim
+   -   *   /   ^   %
```

Binary operators are left-associative: `1 + 2 + 3` evaluates as `(1 + 2) + 3`.

### Comparison

```nim
==   !=   <   <=   >   >=
```

There is no logical AND/OR — `&&`, `||`, `and` and `or` are not recognized by the lexer or parser.

## Function definition

```nim
func funcName(int a, string b) : void
{

}
```

Parameters are declared as `<type> <identifier>`, comma separated. A function that returns a value
ends its body with `return <expression>;` as the last statement, immediately before the closing
`}`:

```nim
func add(int a, int b) : int
{
    return a + b;
}
```

## Control Flow Statements

### If / Else

```nim
if condition
{
    //then body;
}
```

```nim
if condition
{
    //then body;
}
else
{
    //else body;
}
```

Only one `else` per `if` is supported — there is no `elif`/`else if`.

### While Loop

```nim
while condition
{
    //then body;
}
```

### For Loop

```nim
for var i = 0 to 3
{
    //then body;
}
```

The for-loop bound is always `<int declaration> to <int value>`; there is no `foreach`/collection
loop — `foreach` is recognized as a keyword by the lexer, but no parser handles it yet.

## Not yet implemented

The following are referenced by keywords in the lexer, by older examples, or by the standard
library's shape, but have no working syntax in the current parser: `foreach`/`in`, `class`/`new`,
list/array literals (`[...]`) and indexing (`list[index]`), boolean literals, and `elif`. See
[GRAMMAR.md](GRAMMAR.md#not-yet-implemented) for the full list.
