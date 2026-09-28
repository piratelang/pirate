# Pirate Language Grammar (v2)

Status: **canonical**. This document is the single source of truth for the
Pirate v2 lexer, parser, and semantic analyzer (`src-v2/`). v1's historical
grammar documents drifted from what v1 actually implemented; this file
governs v2 only.

Grammar productions use EBNF: `|` alternation, `[ ]` optional, `{ }` zero or
more repetitions, `'x'` a literal token spelling.

For the rationale behind where v2 deliberately differs from what v1's docs
described or implemented (dropped keywords, static typing, arrays, etc.), see
[`GRAMMAR_CHANGES.md`](GRAMMAR_CHANGES.md). The concrete programs in
[`examples/`](examples/) are the reference usage of this grammar.

## 1. Lexical grammar

### 1.1 Whitespace and comments

```
comment    = '//' { any character except newline } (newline | EOF) ;
```
Whitespace (space, tab, newline, carriage return) separates tokens and is
otherwise insignificant. Whitespace and newlines are **not** stripped before
lexing — line/column are tracked per character so diagnostics can point at
real source locations.

### 1.2 Identifiers

```
identifier = letter { letter | digit | '_' } ;
letter     = 'a'..'z' | 'A'..'Z' ;
digit      = '0'..'9' ;
```
An identifier that matches a keyword below is tokenized as that keyword, not
as an identifier.

### 1.3 Keywords

Type keywords: `var` `const` `int` `float` `string` `char` `bool` `void`

Control keywords: `func` `if` `else` `while` `for` `in` `to` `return` `extern`

Module keywords: `import` `export`

Soft keywords: `standard`, `module`, `external`, `as` are ordinary
identifiers that the parser treats as keywords only in the specific
positions of `import-statement` (3.2) — they remain usable as variable and
function names.

Literal keywords: `true` `false`

Reserved, not yet implemented: `class` `new`

### 1.4 Literals

```
int-literal     = digit { digit } ;
float-literal   = digit { digit } '.' digit { digit } ;
string-literal  = '"' { string-part } '"' ;
string-part     = escape-sequence | ( any character except '"' and '\' ) ;
char-literal    = "'" ( escape-sequence | any single character ) "'" ;
bool-literal    = 'true' | 'false' ;
escape-sequence = '\' ( 'n' | 't' | '"' | ''' | '\' ) ;
```

`escape-sequence` decodes to newline, tab, double quote, single quote, and
backslash respectively. A `\` followed by any other character is an error
(`LEX-004` — unknown escape sequence); the lexer records the diagnostic and
keeps the character after the backslash literally. A string or char literal
whose closing quote never arrives before end-of-input is `LEX-002` /
`LEX-003`.

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
return type only — not a value type, cannot be a variable's, parameter's, or
array element's type).

Array types: `T[]` for any scalar type `T` (e.g. `int[]`, `string[]`).
Arrays are single-dimensional; nested arrays (`int[][]`) are not part of this
grammar pass.

### Static typing rules

- Every variable, parameter, and function return has exactly one type,
  fixed at the point of declaration.
- `<type> name = expr;` — `expr` must type-check as `<type>`.
- `var name = expr;` and `const name = expr;` — the variable's type is
  `expr`'s type. `expr` must have a determinable type, so **`var list = [];`
  is invalid** — an empty array literal has no element type to infer. Write
  `int[] list = [];` instead (explicit type required whenever the initializer
  alone doesn't determine one).
- Reassignment (`name = expr;` without a type) requires `expr`'s type to
  match the variable's declared type exactly; no implicit numeric widening
  (`int` does not implicitly convert to `float`).
- `const` makes a variable **immutable**: any later assignment — plain or
  through an index (`list[i] = ...` on a `const` list) — is rejected at
  compile time. All declarations without `const` are mutable subject to the
  exact-type rule above.
- Function calls, operators, and indexing are all checked statically; every
  mismatch is a compile-time diagnostic, never a runtime surprise.

### Operator typing

Operands must have the exact types below — mixed `int`/`float` is an error.
Result types are fixed:

| Operator        | Operand types          | Result |
|-----------------|----------------------|--------|
| `+` `-` `*` `/` | `int`, `int`          | `int` |
| `+` `-` `*` `/` | `float`, `float`      | `float` |
| `+`             | `string`, `string`    | `string` (concatenation) |
| `%` `^`         | `int`, `int`          | `int` |
| `<` `<=` `>` `>=` | `int`, `int` or `float`, `float` | `bool` |
| `==` `!=`       | any two values of the same non-array type | `bool` |
| `&&` `\|\|`     | `bool`, `bool`        | `bool` |
| `!`             | `bool`                | `bool` |
| `-` (unary)     | `int` or `float`      | same type |
| `list[i]`       | `T[]`, index must be `int` | `T` |

The float domains of `%`/`^`, relational ordering of `char`/`string`,
string indexing, and array equality are open items (4).

## 3. Grammar

### 3.1 Program

```
program        = { module-element } ;
module-element = import-statement
               | extern-statement
               | export-statement
               | function-declaration
               | statement ;
```

A module is a list of declarations interleaved with top-level executable
statements. The module the project's fleet manifest names as `entryPoint`
(that is, what `pirate run` executes) is the program: its top-level
statements run, in order. A module with no top-level statements has nothing
to run. Declared functions are visible to the whole module regardless of
declaration order.

Restricting top-level code to the entry module — rejecting statements found
in an *imported* helper module — is part of the module-linking milestone;
until then every module's top-level statements are type-checked as if they
will run, and `pirate build` accepts declaration-only and statement-bearing
modules alike.

### 3.2 Imports and externs

```
import-statement = 'import' 'standard' qualified-name ';'
                 | 'import' import-source qualified-name [ 'as' identifier ] ';' ;
import-source    = 'module' | 'external' ;
extern-statement = 'extern' qualified-name ';' ;
qualified-name   = identifier { '.' identifier } ;
```

- `import standard Terminal;` binds **every builtin in that standard
  namespace group** into the module's global scope — under its leaf name
  (`Print`, `PrintLine`, `Read`) *and* its full dotted path
  (`Standard.Terminal.Print`), so imported and extern-imported functions
  behave identically. The group is `Standard.<name>`, matched against the
  builtin registry; an unknown group is a compile-time error.
- `extern Standard.Terminal.Print;` imports a single builtin by its exact
  dotted path. It is the low-level form behind the same mechanism; mixing
  both for the same function is a duplicate declaration.
- Only module and external imports take an alias (`as <Name>` is rejected
  with SYN-049 after `standard`, which binds the group's own names).
- `import module data as Data;` and `import external data as Data;` are
  defined here but **not yet implemented**: the analyzer rejects them until
  the module linker (cross-module resolution against the fleet manifest)
  lands. The optional `alias` is the name the imported module's exports are
  reached through (`Data.data`); without it the last path segment is used.

### 3.3 Exports

```
export-statement = 'export' ( function-declaration | variable-declaration ) ;
```

`export` marks a top-level declaration as visible to modules that import
this one. Until the linker exists, exported declarations compile exactly
like unmarked ones; the marker is recorded and unused.

### 3.4 Function declaration

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

`return` with no expression is only valid in a `void`-returning function,
and a `void` function must not return a value. `return` may appear as the
block's final statement; a `return` outside any function body (a top-level
statement) is rejected. The semantic pass rejects a non-`void` function
without a `return` covering every path: either the block ends in a
`return`, or its last statement is an if/else whose every branch returns
on all paths.

### 3.5 Statements

```
statement         = variable-declaration
                  | assignment-statement
                  | if-statement
                  | while-statement
                  | for-statement
                  | expression-statement ;

variable-declaration
                  = 'const' [ type | 'var' ] identifier '=' expression ';'
                  | ( type | 'var' ) identifier '=' expression ';' ;

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

A declaration without `const` requires a type or `var` so that `x = 5;`
remains unambiguously an assignment; with `const`, the type is optional —
omitted, it is inferred from the initializer (and must be determinable, as
with `var`).

`if`'s and `while`'s condition is any `bool`-typed expression — no
parentheses required, though a parenthesized expression is still valid since
`( expr )` is a primary expression (3.6).

`for-in` iterates `expression`, which must type-check as `T[]` for some `T`;
`identifier` is bound with type `T` for the loop body. `for-counting` binds
its `identifier` as a mutable `int` and requires both bounds to be `int`.

An `expression-statement` must be a function call; a value expression
discarded this way is a compile-time error.

### 3.6 Expressions

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

`postfix` covers both function calls (`name(args)`,
`Standard.Terminal.Print(x)`) and array indexing (`list[i]`), including
chained forms like `f()[0]`.

An array literal's elements must all share one type; the empty literal `[]`
takes its element type from the declared type of the variable it initializes
(see 2, so it is valid only in a typed-declaration position).

## 4. Open items (explicitly out of scope for this pass)

- `elif` sugar.
- `class` / `new` (reserved keywords, no grammar defined).
- Multi-dimensional / nested arrays.
- Implicit numeric conversions (`int` → `float`).
- Module linking: resolving `import module` / `import external`, export
  visibility across modules, and rejecting top-level statements in imported
  helper modules (3.1–3.3).
- Variadic builtins (v1's `Standard.String.Concat` took any number of
  strings; v2 currently declares the two-string form).
- `%` and `^` on `float`; relational ordering of `char` and `string`;
  indexing into `string` (e.g. `s[0]` → `char`); array equality with `==`.
- Block scoping for `if`/`while`/`for` bodies — today a function (or module)
  is one scope, so a loop variable stays visible after the loop.
- Closure / free-variable capture (reserved symbol scope `Free`).
