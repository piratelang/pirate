# Pirate Language Grammar (v2)

Status: **canonical**. This document is the single source of truth for the
Pirate v2 lexer, parser, and semantic analyzer (`src-v2/`). It supersedes the
root [`GRAMMAR.md`](../GRAMMAR.md) and [`SYNTAX.md`](../SYNTAX.md), which
described v1 (`src/`) and had drifted from what v1 actually implements and
from each other. v1 keeps its own docs untouched; this file only governs v2.

Grammar productions use EBNF: `|` alternation, `[ ]` optional, `{ }` zero or
more repetitions, `'x'` a literal token spelling.

For the rationale behind where v2 deliberately differs from what v1's docs
described or implemented (dropped keywords, static typing, arrays, etc.), see
[`GRAMMAR_CHANGES.md`](GRAMMAR_CHANGES.md).

## 1. Lexical grammar

### 1.1 Whitespace and comments

```
comment    = '//' { any character except newline } (newline | EOF) ;
```
Whitespace (space, tab, newline, carriage return) separates tokens and is
otherwise insignificant. Unlike v1, whitespace and newlines are **not**
stripped before lexing — line/column are tracked per character so diagnostics
can point at real source locations.

### 1.2 Identifiers

```
identifier = letter { letter | digit | '_' } ;
letter     = 'a'..'z' | 'A'..'Z' ;
digit      = '0'..'9' ;
```
An identifier that matches a keyword below is tokenized as that keyword, not
as an identifier.

### 1.3 Keywords

Type keywords: `var` `int` `float` `string` `char` `bool` `void`

Control keywords: `func` `if` `else` `while` `for` `in` `to` `return` `extern`

Literal keywords: `true` `false`

Reserved, not yet implemented: `class` `new`

### 1.4 Literals

```
int-literal    = digit { digit } ;
float-literal  = digit { digit } '.' digit { digit } ;
string-literal = '"' { any character except '"' } '"' ;
char-literal   = "'" any single character "'" ;
bool-literal   = 'true' | 'false' ;
```

### 1.5 Operators and punctuation

| Category    | Tokens |
|-------------|--------|
| Assignment  | `=` |
| Arithmetic  | `+` `-` `*` `/` `%` `^` |
| Comparison  | `==` `!=` `<` `<=` `>` `>=` |
| Logical     | `&&` `\|\|` `!` |
| Grouping    | `(` `)` `{` `}` `[` `]` |
| Separators  | `,` `:` `;` `.` |

## 2. Types

Scalar types: `int`, `float`, `string`, `char`, `bool`, `void` (function
return type only — not a value type, cannot be a variable's type).

Array types: `T[]` for any scalar type `T` (e.g. `int[]`, `string[]`).
Arrays are single-dimensional; nested arrays (`int[][]`) are not part of this
grammar pass.

### Static typing rules

- Every variable, parameter, and function return has exactly one type,
  fixed at the point of declaration.
- `<type> name = expr;` — `expr` must type-check as `<type>`.
- `var name = expr;` — the variable's type is `expr`'s type. `expr` must have
  a determinable type, so **`var list = [];` is invalid** — an empty array
  literal has no element type to infer. Write `int[] list = [];` instead
  (explicit type required whenever the initializer alone doesn't determine
  one).
- Reassignment (`name = expr;` without a type) requires `expr`'s type to
  match the variable's declared type exactly; no implicit numeric widening
  (`int` does not implicitly convert to `float`).

## 3. Grammar

### 3.1 Program

```
program           = { top-level-statement } ;
top-level-statement
                  = extern-statement
                  | function-declaration ;
```

A valid program must define a zero-parameter `func main() : void { ... }` as
its entry point (as in v1).

### 3.2 Extern

```
extern-statement  = 'extern' qualified-name ';' ;
qualified-name    = identifier { '.' identifier } ;
```
Declares a standard-library function available by its dotted path, e.g.
`extern Standard.Terminal.Print;`.

### 3.3 Function declaration

```
function-declaration
                  = 'func' identifier '(' [ parameter-list ] ')' ':' type
                    block ;
parameter-list    = parameter { ',' parameter } ;
parameter         = type identifier ;
type              = scalar-type [ '[' ']' ] ;
scalar-type       = 'int' | 'float' | 'string' | 'char' | 'bool' | 'void' ;
block             = '{' { statement } [ return-statement ] '}' ;
return-statement  = 'return' [ expression ] ';' ;
```
`return` with no expression is only valid in a `void`-returning function.
`return` may appear as the block's final statement (matching v1's shape of
"statements, then one return"); v2's semantic pass additionally rejects a
non-`void` function with no reachable `return`.

### 3.4 Statements

```
statement         = variable-declaration
                  | assignment-statement
                  | if-statement
                  | while-statement
                  | for-statement
                  | expression-statement ;

variable-declaration
                  = ( type | 'var' ) identifier '=' expression ';' ;

assignment-statement
                  = identifier [ '[' expression ']' ] '=' expression ';' ;

if-statement      = 'if' expression block [ 'else' ( block | if-statement ) ] ;

while-statement   = 'while' expression block ;

for-statement     = for-counting | for-in ;
for-counting      = 'for' 'var' identifier '=' expression 'to' expression block ;
for-in            = 'for' '(' identifier 'in' expression ')' block ;

expression-statement
                  = expression ';' ;
```

`if`'s and `while`'s condition is any `bool`-typed expression — no
parentheses required (matching v1), though a parenthesized expression is
still valid since `( expr )` is a primary expression (3.5).

`for-in` iterates `expression`, which must type-check as `T[]` for some `T`;
`identifier` is bound with type `T` for the loop body.

### 3.5 Expressions

Precedence, lowest to highest (each level left-associative unless noted):

```
expression        = logical-or ;
logical-or        = logical-and { '||' logical-and } ;
logical-and       = equality { '&&' equality } ;
equality          = relational { ( '==' | '!=' ) relational } ;
relational        = additive { ( '<' | '<=' | '>' | '>=' ) additive } ;
additive          = multiplicative { ( '+' | '-' ) multiplicative } ;
multiplicative    = power { ( '*' | '/' | '%' ) power } ;
power             = unary { '^' unary } ;            (* right-associative *)
unary             = ( '!' | '-' ) unary | postfix ;
postfix           = primary { call-suffix | index-suffix } ;
call-suffix       = '(' [ argument-list ] ')' ;
index-suffix      = '[' expression ']' ;
argument-list     = expression { ',' expression } ;
primary           = int-literal | float-literal | string-literal
                  | char-literal | bool-literal
                  | array-literal
                  | qualified-name
                  | '(' expression ')' ;
array-literal     = '[' [ expression { ',' expression } ] ']' ;
```

`postfix` covers both function calls (`name(args)`, `Standard.Terminal.Print(x)`)
and array indexing (`list[i]`), including chained forms like `f()[0]`.

## 4. Open items (explicitly out of scope for this pass)

- `elif` sugar.
- `class` / `new` (reserved keywords, no grammar defined).
- Multi-dimensional / nested arrays.
- Implicit numeric conversions (`int` → `float`).
