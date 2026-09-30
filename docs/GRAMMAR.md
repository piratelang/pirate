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

Module keywords: `import`

Visibility keywords: `private` (replaces `export` — see 3.3 and
[GRAMMAR_CHANGES.md](GRAMMAR_CHANGES.md))

Class keywords: `field` `constructor` `readonly` `self` `null`. `new` is
promoted from "reserved" to an expression keyword (3.6, 4).

Soft keywords: `standard`, `module`, `external`, `as` are ordinary
identifiers that the parser treats as keywords only in the specific
positions of `import-statement` (3.2) — they remain usable as variable and
function names.

Literal keywords: `true` `false`

Reserved, not yet implemented: `class` `super` `extends` `implements`
`abstract` `override` `static` (see 4 for why each is reserved).

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

Array types: `T[]` for any scalar type `T` (e.g. `int[]`, `string[]`), or for
a class type once classes exist (`Item[]`, 4). Arrays are single-dimensional;
nested arrays (`int[][]`) are not part of this grammar pass.

Class types (4) are named by their file's name and can appear anywhere a
scalar type can: variable/field/parameter/return types, and array element
types.

### Nullable types

Every type, scalar or class, has a nullable form: `T?` (`int?`, `Item?`,
`Item[]?`, `Item?[]`). A plain `T` is never null; `T` is a subtype of `T?`.
`void` is never nullable. `null` is the literal for the empty value of any
`T?`.

- `== null` / `!= null` work on every type, including arrays, with flow
  narrowing to `T` inside the true branch of `!= null` (and the false branch
  of `== null`). Narrowing applies to local variables and parameters, not to
  fields — a field may change between the check and the use, so copy it into
  a local first.
- A nullable value can't be used as its base type until narrowed: `int? a; a
  + 1;` is a compile-time error.
- `?.`, `??`, and `x!` are not part of this grammar pass (5).

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
| `==` `!=`       | `T?`/`T`, `null`      | `bool` (any type, 2) |
| `==` `!=`       | two class values      | `bool` (reference equality) |

The float domains of `%`/`^`, relational ordering of `char`/`string`,
string indexing, and array equality are open items (5).

## 3. Grammar

### 3.1 Program

```
program        = { module-element } ;
module-element = import-statement
               | extern-statement
               | [ 'private' ] function-declaration
               | [ 'private' ] variable-declaration
               | statement ;
```

A `.pirate`/`.pir` module is a list of declarations interleaved with
top-level executable statements. The module the project's fleet manifest
names as `entryPoint` (that is, what `pirate run` executes) is the program:
its top-level statements run, in order. A module with no top-level
statements has nothing to run. Declared functions are visible to the whole
module regardless of declaration order.

Every top-level declaration is public by default; `private` hides it from
importers (3.3). A **non-entry** module holds functions and `const` values
only — a mutable module-level variable (`var`, or a typed declaration
without `const`) at top level of a non-entry module is a compile-time error,
so there is no shared mutable state across files (4).

Restricting top-level statements to the entry module — rejecting them in an
*imported* helper module — is part of the module-linking milestone (Phase 3,
[`brainstorm/FLAT_PLAN.md`](brainstorm/FLAT_PLAN.md)); until then every
module's top-level statements are type-checked as if they will run, and
`pirate build` accepts declaration-only and statement-bearing modules alike.

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

### 3.3 Visibility

```
visibility-modifier = 'private' ;
```

`export` is removed (see [GRAMMAR_CHANGES.md](GRAMMAR_CHANGES.md)). Every
top-level declaration — in a module or a class file — is **public by
default**; a leading `private` hides it from other files instead. Until the
linker exists, the modifier is recorded and unused; once it lands,
`private` on a top-level module function/`const` hides it from importers the
same way `private` hides a class member (4) from other files.

### 3.4 Function declaration

```
function-declaration
                  = 'func' identifier '(' [ parameter-list ] ')' ':' type
                    block ;
parameter-list    = parameter { ',' parameter } ;
parameter         = type identifier ;
type              = base-type [ '?' ] [ '[' ']' [ '?' ] ] ;
base-type         = scalar-type | qualified-name ;     (* qualified-name = a class type, 4 *)
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
postfix           = primary { call-suffix | index-suffix | member-suffix } ;
call-suffix       = '(' [ argument-list ] ')' ;
index-suffix      = '[' expression ']' ;
member-suffix     = '.' identifier ;                (* c.value(), c.count — 4 *)
argument-list     = expression { ',' expression } ;
primary           = int-literal | float-literal | string-literal
                  | char-literal | bool-literal
                  | array-literal
                  | qualified-name
                  | 'self' | 'null' | new-expression
                  | '(' expression ')' ;
array-literal     = '[' [ expression { ',' expression } ] ']' ;
new-expression    = 'new' qualified-name '(' [ argument-list ] ')' ;
```

`postfix` covers function calls (`name(args)`, `Standard.Terminal.Print(x)`),
array indexing (`list[i]`), and member access (`c.value()`, `c.count`, 4),
including chained forms like `f()[0]` or `c.value().add(1)`.

`self` is the current instance inside a class file's constructor or method
body (4); it is never a type. `null` is the nullable-empty literal (2).

An array literal's elements must all share one type; the empty literal `[]`
takes its element type from the declared type of the variable it initializes
(see 2, so it is valid only in a typed-declaration position).

## 4. Classes and namespaces (flat files)

Status: **specified, not yet implemented** (see
[`brainstorm/FLAT_PLAN.md`](brainstorm/FLAT_PLAN.md) for the build order —
Phases 1–6 land the lexer, parser, semantics, compiler/VM, and stdlib
support this section assumes). [`brainstorm/FLAT.md`](brainstorm/FLAT.md) is
the fuller prose walkthrough with worked examples; this section is its
grammar folded into the canonical spec, condensed to production rules and
the rules that go with them. First slice only: classes (fields,
constructors, methods, `self`, namespaces, imports, nullable types).
`extends`, `implements`, interfaces and `static` are reserved but not
defined here (5).

**A file is a type, and the filename is the type's name.** There is no
`class Foo { }` wrapper — everything at the top level of a class file is a
member of that type. The filename without its final extension is the type
name and must be a single identifier (`foo.bar.cpirate` is an error, not a
type named `foo`).

| Extensions | Kind | Contains |
|---|---|---|
| `.cpirate`, `.cpir` | class | fields, constructors, methods |
| `.ipirate`, `.ipir` | interface | bodiless method signatures (reserved, 5) |
| `.pirate`, `.pir` | module | functions and constants (3.1) |

Two files that resolve to the same type name in one folder are a
duplicate-name error, whatever their extensions (`Stack.cpir` next to
`Stack.cpirate`, or a class `Stack.cpir` next to a module `Stack.pir`).

### 4.1 Class-file grammar

```
class-file        = { class-element } ;
class-element     = { modifier } ( field | method | constructor ) ;
modifier          = 'private' | 'readonly' ;
                  (* 'readonly' on fields only; 'override'/'abstract' reserved, 5 *)

field             = 'field' type identifier [ '=' expression ] ';'
                  | 'const' [ type | 'var' ] identifier '=' expression ';' ;
                  (* on 'field', the initializer may be omitted only if every
                     constructor assigns the field (definite assignment); a
                     nullable field is never implicitly null *)
method            = 'func' identifier '(' [ parameter-list ] ')' ':' type block ;
constructor       = 'constructor' '(' [ parameter-list ] ')'
                    [ ':' delegate ] block ;
delegate          = 'self' '(' [ argument-list ] ')' ;
                  (* 'super(...)' reserved until 'extends' exists, 5 *)
```

A class file has no loose statements — only fields, constructors, methods,
and constants. `field var label = "x";` infers the field's type from its
initializer, same rule as a local `var`. A member can't share the file's own
name, since the file's name is in scope as a type throughout the body.

### 4.2 Access levels

Exactly three, applying to fields, methods, and constructors alike (for
methods/constructors there's nothing to write, so `readonly` on them — or on
a `const` — is an error):

| Written | Read from other files | Assigned from other files | Assigned inside the class |
|---|---|---|---|
| `field int count` | yes | yes | yes |
| `readonly field int count` | yes | no | yes |
| `private field int count` | no | no | yes |

"Inside the class" means the class's own constructors and methods.
`readonly` doesn't make the *value* immutable — the class can still change
it — only who can assign the field from outside.

### 4.3 Overloading and assignment

Constructors and methods overload by arity, then parameter types;
ambiguity is a compile-time error. Assignment targets extend through member
access: `self.count = 1;`, `c.count = 1;` (subject to 4.2).

### 4.4 Namespaces and resolution

The `.fleet` file's base name is the root namespace; folders are
sub-namespaces (lowercase by convention); the filename is the type
(PascalCase by convention) — no `namespace` keyword, the filesystem is the
declaration. A type's full name is root + folder path + file name
(`shop.models.Money`).

- Declarations in the same folder — classes and modules alike — are visible
  without an import. Anything else needs
  `import module <full name> [as <alias>];`; the imported name is the last
  segment, or the alias. `import external <dep>.<path> [as <alias>];`
  resolves `<dep>` against the `.fleet` file's `dependencies` map
  ([`FLEET.md`](FLEET.md)).
- A full name works without an import (`shop.models.Item i = ...`),
  resolving by longest namespace prefix, then type, then member.
- Two files with the same full name are an error; a type can't share its
  name with a sibling folder (`Money.cpirate` and `Money/`). Names and
  folders compare case-insensitively.
- Circular imports are allowed — every type is registered before any body
  is checked.

## 5. Open items (explicitly out of scope for this pass)

- `elif` sugar.
- `extends`, `implements`, `.ipirate` interfaces, virtual dispatch,
  `abstract`/`override`, `static` members — reserved keywords (1.3), no
  grammar defined. Tracked as backlog after the classes first slice
  ([`brainstorm/FLAT_PLAN.md`](brainstorm/FLAT_PLAN.md), "After the first
  slice").
- Nullable operators `?.`, `??`, `x!` (2).
- Multi-dimensional / nested arrays.
- Implicit numeric conversions (`int` → `float`).
- Module linking: resolving `import module` / `import external`, visibility
  across modules, and rejecting top-level statements in imported helper
  modules (3.1, 3.3, 4.4) — Phase 3 of the flat-files plan.
- Variadic builtins (v1's `Standard.String.Concat` took any number of
  strings; v2 currently declares the two-string form).
- `%` and `^` on `float`; relational ordering of `char` and `string`;
  indexing into `string` (e.g. `s[0]` → `char`); array equality with `==`
  (`T[]`/`T[]`, as opposed to the `== null` form in 2, which is in scope).
- Block scoping for `if`/`while`/`for` bodies — today a function (or module)
  is one scope, so a loop variable stays visible after the loop. Affects how
  4's nullable narrowing and field definite-assignment interact with blocks.
- Closure / free-variable capture (reserved symbol scope `Free`).
- Enums and exhaustive `match`; generics (Pirate has none, out of scope).
