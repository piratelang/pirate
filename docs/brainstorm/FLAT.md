# Pirate Flat Files (brainstorm)

Status: **brainstorm, not canonical.** Nothing here is implemented. If adopted,
the grammar moves into `docs/GRAMMAR.md` and the decisions into
`docs/design/DESIGN_DECISIONS.md`. The build order is in
[`FLAT_PLAN.md`](FLAT_PLAN.md).

**First slice:** classes only, meaning fields, constructors, methods, `self`,
namespaces, imports and nullable types. `extends`, `implements`, interfaces and
`static` come later. Examples below that use them are marked.

The idea: **a file is a type, and the filename is the type's name.** There is no
`class Foo { ... }` wrapper, so everything at the top level of a class file is a
member of that type. Only the entry file may contain loose statements.

Pirate already works this way for modules (declarations at the top level, loose
statements only in the entry module). Flat files add types, instances, and an
extension that says what kind of file it is.

## File kinds

The extension says what kind of file it is. Each kind has a full spelling and a
short one, and the tooling accepts both. The type name is the filename without
its final extension, and it must be a single identifier: `foo.bar.cpirate` is an
error, not a type named `foo`.

| Extensions | Kind | Contains |
|---|---|---|
| `.cpirate`, `.cpir` | class | fields, constructors, methods |
| `.ipirate`, `.ipir` | interface | bodiless method signatures (later milestone) |
| `.pirate`, `.pir` | module | functions and constants (today's helper module) |

Examples: `Stack.cpirate` (or `Stack.cpir`), `Shape.ipirate`, `data.pirate` (or
`data.pir`). The old `.class.pirate` spelling is not used.

The `c` and `i` prefixes are part of the extension, not the name, so `Stack.cpir`
is the type `Stack`. Two files that resolve to the same type name in one folder
are a duplicate-name error, whatever their extensions: `Stack.cpir` next to
`Stack.cpirate`, or a class `Stack.cpir` next to a module `Stack.pir`.

| Entry | Kind | Contains |
|---|---|---|
| `main.pirate` or `main.pir` (the manifest's `entryPoint`) | entry | loose statements; the program |

The entry file keeps the existing rule: whichever module the fleet manifest names
as `entryPoint` is the program. By default it is `main.pirate` in the project
root, and the `entryPoint` field in the `.fleet` file overrides that with a path
relative to the root, without the extension. It must be a module file — naming a
`.cpirate`, `.cpir`, `.ipirate` or `.ipir` file as `entryPoint` is a manifest
error. If both `main.pirate` and `main.pir` exist, the entry point is ambiguous
and is an error too.

Inside a class file (`.cpirate` or `.cpir`):

- `self` is lowercase, like every other keyword, and has two uses:
  - as a constructor delegate it calls another constructor:
    `constructor() : self(16) { }`;
  - in an expression it is the current instance: `self.cents = cents;`.
- `self` is never a type. The type is spelled by the file name, inside the
  file and out: `func add(Counter other) : Counter`, `new Counter(10)`.
  Renaming the file is a refactor that touches the body and every importer;
  that is tooling's job, not a language workaround.
- A member can't have the same name as the file, because the file's name is
  in scope as a type throughout the body.

## Getting started

Create a project with three files.

**`Counter.cpirate`**

```
const int Step = 1;

readonly field int count = 0;
private field int calls = 0;

constructor() { }

constructor(int start) {
    count = start;
}

func increment() : void {
    count = count + Step;
    calls = calls + 1;
}

func value() : int {
    return count;
}

private func reset() : void {
    count = 0;
}
```

**`MathUtil.pirate`**

```
func square(int x) : int {
    return x * x;
}
```

**`main.pirate`** (the entry file)

```
import standard Terminal;
import module Counter;
import module MathUtil;

Counter c = new Counter(10);
c.increment();
c.increment();

PrintLine(MathUtil.square(c.value()));
```

Run it:

```bash
pirate run
```

Reading the example:

- `Counter.cpirate` has no wrapper. The fields, constructors and methods
  are all top-level, so they are all part of `Counter`.
- Everything is **public by default**. `private` hides a member from other
  files, and `readonly` lets other files read a field but not assign it.
  `c.count` and `c.increment()` work from `main.pirate`, while `c.count = 5;`,
  `c.calls` and `c.reset()` are compile errors.
- Constructors are chosen by argument count and types, so `new Counter(10)`
  calls the second one.
- `main.pirate` is the only file with loose statements. A loose statement in
  `Counter.cpirate` or `MathUtil.pirate` is a compile error.

## Declaring members

```
field int count = 0;                // field
private field int[] items = [];     // private field
const int Limit = 100;              // constant: needs no `field`, it cannot change
field var label = "x";              // inferred type, same rules as locals
field Discount? discount = null;    // nullable, but still assigned
func push(int item) : void { ... }  // method
constructor(int capacity) { ... }   // constructor
```

- **`field` is required** on every field in a class file. A bare
  `int count = 0;` at the top level of a class file is a loose statement, and so
  a compile error. The keyword is what separates a field from a stray
  declaration. `member` stays ordinary English in prose — fields, methods and
  constructors are the class's members — but no keyword uses it.
- Methods use `func`, as they do now.
- Constructors use `constructor`. The keyword `new` is used only to create an
  instance: `new Counter(10)`.
- Modifiers come before the member: `private`, `readonly`, and later `override`
  and `abstract`. `static` is reserved but rejected in the first slice, because
  shared functions and constants live in modules.
- **Three access levels, and that is all in the first slice.** A field is one of:

  | Written | Read from other files | Assigned from other files | Assigned inside the class |
  |---|---|---|---|
  | `field int count` | yes | yes | yes |
  | `readonly field int count` | yes | no | yes |
  | `private field int count` | no | no | yes |

  "Inside the class" means the class's constructors and methods. `readonly` is
  for fields only, and it doesn't make the value immutable: the class can still
  change it, and a `readonly` field holding an object still lets anyone call
  that object's public methods. `private` and public also apply to methods and
  constructors, where there is nothing to write, so `readonly` on them is an
  error. `readonly const` is an error too, since a constant can't be assigned at
  all.
- Methods can be overloaded by arity and parameter types, under the same rules
  as constructors.
- A constant in a class file is `const int Limit = 1;`, without `field`, as it
  is everywhere else in Pirate.
- **Every field must get a value, nullable or not.** There are no implicit
  defaults, so a nullable field is not silently `null`. Either it has `= expr` at
  its declaration (`= null` for a nullable field), or every constructor assigns
  it before the body ends. A constructor that delegates with `: self(...)`
  counts as assigning whatever the target constructor assigns.
- Field initializers run in declaration order, before the constructor body. An
  initializer can't read a later field or call a method on `self`.
- **Public by default.** `private` is the only visibility keyword. It replaces
  `export` everywhere, including plain modules (see "Changes to existing
  grammar").

## Nullable types

Every type has a nullable form: `T?`. That covers scalars (`int?`, `string?`),
classes (`Item?`) and arrays (`Item[]?`, `Item?[]`). A plain `T` is never null.
`null` is the literal, and `T` is a subtype of `T?`.

```
Item? found = null;
if found != null {
    PrintLine(found.name);      // narrowed to Item inside the block
}
```

- The first slice has `null`, `== null`, `!= null` and flow narrowing.
  `?.`, `??` and `x!` are a later addition.
- A nullable value can't be used as its base type until it is narrowed. `int? a;
  a + 1` is a compile error.
- Narrowing applies to local variables and parameters. It does not apply to
  fields, because a field may change between the check and the use. Copy the
  field into a local first.
- Because misuse is a compile error, there is no run-time null failure to
  report.

## Namespaces and the fleet file

The `.fleet` file's base name is the root namespace, folders are
sub-namespaces, and the filename is the type. The filesystem is the only
namespace declaration, so there is no `namespace` keyword.

`pirate init` prompts for the project name. That name is both the `.fleet` file
name and the root namespace, so `pirate init` no longer defaults to
`module.fleet`. The existing `-n|--name` option supplies the name without the
prompt, which scripts and tests need. The `name` field in the file is a display
label and doesn't affect resolution. Reserved roots such as `standard` are
refused.

The example below includes an interface and `implements`, which arrive after the
first slice. In the first slice, drop `Discount.ipirate`, and let `Cart`
take a `Percent` directly.

```
shop/
├── shop.fleet                      root namespace: shop
├── main.pirate                     entry: shop.main
├── Cart.cpirate               shop.Cart
├── models/
│   ├── Item.cpirate           shop.models.Item
│   └── Money.cpirate          shop.models.Money
└── pricing/
    ├── Discount.ipirate   shop.pricing.Discount
    ├── Percent.cpirate        shop.pricing.Percent
    └── Rounding.pirate             shop.pricing.Rounding (module of functions)
```

**`shop.fleet`**

```json
{
  "name": "shop",
  "version": "0.1.0",
  "entryPoint": "main",
  "build": {},
  "dependencies": {
    "geometry": "../geometry"
  }
}
```

`entryPoint` is relative to the root, so `"main"` means `shop.main`. Each
dependency is another project, and its `.fleet` file name is that project's root
namespace.

**`models/Money.cpirate`**

```
field int cents = 0;

constructor(int cents) {
    self.cents = cents;
}

func add(Money other) : Money {
    return new Money(cents + other.cents);
}
```

**`models/Item.cpirate`**: `Money` is in the same folder, so no import.

```
field string name = "";
field Money price = new Money(0);
private field int stock = 0;

constructor(string name, Money price) {
    self.name = name;
    self.price = price;
}
```

**`pricing/Discount.ipirate`**

```
import module shop.models.Money;

func apply(Money total) : Money;
```

**`pricing/Percent.cpirate`**: `Discount` and `Rounding` are same-folder.

```
import module shop.models.Money;

implements Discount;

field int percent = 0;

constructor(int percent) {
    self.percent = percent;
}

func apply(Money total) : Money {
    return new Money(Rounding.roundDown(total.cents * (100 - percent) / 100));
}
```

**`pricing/Rounding.pirate`** is a module of plain functions.

```
func roundDown(int cents) : int {
    return cents - cents % 5;
}
```

**`Cart.cpirate`**

```
import module shop.models.Item;
import module shop.models.Money;
import module shop.pricing.Discount;

field Item[] items = [];
private field Discount? discount = null;

constructor() { }

func add(Item item) : void { ... }
func setDiscount(Discount d) : void { discount = d; }
func total() : Money { ... }
```

**`main.pirate`**

```
import standard Terminal;
import module shop.models.Item;
import module shop.models.Money as Cash;
import module shop.pricing.Percent;
import external geometry.shapes.Circle;

Cart cart = new Cart();
cart.add(new Item("Pirate hat", new Cash(2500)));
cart.setDiscount(new Percent(10));

PrintLine(cart.total().format());
```

### Resolution rules

1. A type's full name is the root namespace, its folder path, then its file
   name (`shop.models.Money`). Folders and namespaces are lowercase by
   convention, types are PascalCase.
2. Declarations in the same folder — classes and modules alike — see each
   other without an import. Anything else needs `import module <full name>;`,
   with an optional `as` alias. The imported name is the last segment, or the
   alias.
3. Files in the project root belong to the root namespace.
4. `import external geometry.shapes.Circle;` resolves `geometry` against the
   `dependencies` map, and the rest is a path inside that project.
5. A full name works without an import: `shop.models.Item i = ...`. A dotted
   name resolves by longest namespace prefix, then type, then member.
6. Two files with the same full name are an error, and a type can't share its
   name with a sibling folder (`Money.cpirate` and `Money/`). Names and
   folders are compared case-insensitively, because Windows and macOS
   filesystems cannot always tell `Money` from `money`.
7. Circular imports are allowed, because all types are registered before bodies
   are checked. Circular `extends` and circular constructor delegation are
   errors.

## Grammar sketch

Extends the notation in `docs/GRAMMAR.md`. Productions not listed there are
unchanged.

```
class-file        = { class-header } { class-element } ;
class-header      = import-statement
                  | extends-clause
                  | implements-clause ;
extends-clause    = 'extends' qualified-name ';' ;
implements-clause = 'implements' qualified-name { ',' qualified-name } ';' ;

class-element     = { modifier } ( field | method | constructor ) ;
modifier          = 'private' | 'readonly' | 'override' | 'abstract' ;
                  (* 'readonly' on fields only; 'static' reserved, rejected *)

field             = 'field' ( type | 'var' ) identifier [ '=' expression ] ';'
                  | 'const' [ type | 'var' ] identifier '=' expression ';' ;
                  (* on `field`, the initializer may be omitted only if every
                     constructor assigns the field (definite assignment); a
                     nullable field is never implicitly null *)
method            = 'func' identifier '(' [ parameter-list ] ')' ':' type
                    ( block | ';' ) ;                       (* ';' = abstract *)
constructor       = 'constructor' '(' [ parameter-list ] ')'
                    [ ':' delegate ] block ;
delegate          = ( 'self' | 'super' ) '(' [ argument-list ] ')' ;

type              = base-type [ '?' ] [ '[' ']' [ '?' ] ] ;
base-type         = scalar-type | qualified-name ;          (* no 'self' type; 'void' is never nullable *)

(* expression additions *)
primary          += 'self' | 'null' | new-expression ;      (* 'self' = the current instance *)
new-expression    = 'new' qualified-name '(' [ argument-list ] ')' ;
postfix          += member-suffix ;
member-suffix     = '.' identifier ;                        (* c.value(), c.count *)
```

### Changes to existing grammar

- `export-statement` (3.3) is removed. Top-level declarations in a module are
  public by default, and `private` hides one. Until the linker exists, both are
  recorded and unused.
- New reserved words: `field`, `constructor`, `private`, `readonly`, `static`,
  `override`, `abstract`, `extends`, `implements`, `self`, `super`, `null`. `class` stays
  reserved but is unused, since the file kind carries it.
- Types gain `?` and can be class names, and arrays of classes are allowed
  (`docs/GRAMMAR.md` §2 limits arrays to scalars today).
- `new` moves from "reserved" to "expression keyword".

### Rules by file kind

1. Class file: no loose statements. Fields, constructors, methods and
   constants only. A method with no body is abstract, and a class with any
   abstract method can't be instantiated.
2. Module file (not the entry): functions and `const` values only. A mutable
   module-level variable is an error, so there is no shared global state across
   files. No `constructor`, `field`, `extends`, `implements` or `self`.
3. Entry file: loose statements allowed, as today.
4. One `extends`, any number of `implements`.
5. Constructors are overloaded by arity and parameter types. Two constructors
   with the same signature are a compile error.
6. A constructor with no `: delegate` implicitly calls `super()`.
7. Method overloads follow the same rule as constructors: overload by arity,
   then parameter types, and ambiguity is a compile error.
8. `import module Foo;` resolves `Foo` to a class, interface or module file
   named `Foo` (any of its extensions) in the project. The imported name is the last path segment, or the `as` alias.
9. No nested types in the first version.
10. `extends`, `implements`, `abstract`, `override` and `static` are reserved
    but not supported in the first slice. The parser reports one dedicated
    "not supported yet" error for each, rather than a generic syntax error.
11. `== null` and `!= null` work on every type, including arrays. `==` and `!=`
    between two class values compare references. Equality between two arrays
    stays undefined (issue #214).

## Decisions

**Fields use `field`, and it is required.** In a class file every top-level
line is a declaration, so parsing would work without a keyword. The keyword is
chosen for readability and tooling: a field is visibly a field, a bare declaration
in a class file is an error rather than a silent field, and fields are easy to
search for. `member` was the first choice and is rejected: in the OOP glossary
methods and constructors are members too, so a field-only keyword called
`member` misnames the rest of the class file's contents. `prop` stays free for
a later accessor feature.

**`self` is the instance and the constructor delegate, never a type.** Inside
a class file the type is spelled by the file name, like anywhere else, so a
signature means the same thing on both sides of the file boundary, interfaces
need no `self` rule, and one spelling never has two meanings. Renaming a file
then touches its body — a refactor tooling handles, and cheaper than the
per-position ambiguity the alternative would need rules for.

**Constructors use `constructor`.** It reads as the counterpart of `func`. It
also frees `new` to mean only "create an instance", so a definition and a call
never share a keyword.

**Public by default, `private` opts out, `readonly` splits read from write.**
One rule for classes and modules, no `export` keyword. Read and write access are
managed per field with exactly three levels: public, `readonly` and `private`.
Independent `private read` / `private write` control, `protected`, and accessor
blocks (`prop`) are later additions.

**Naming is convention only.** Folders and namespaces are lowercase and types
are PascalCase, documented in `docs/STYLE.md` and not enforced. Names that
differ only by case are still a collision error.

**The kind lives in the extension.** `.cpirate` and `.ipirate` extend `.pirate`
by one letter, and `.cpir`, `.ipir` and `.pir` are the short spellings, so the
language keeps one family of extensions and existing `.pirate` files stay valid.
Tooling knows the kind from the filename before parsing. The `.class.pirate`
spelling was rejected: it made the type name depend on counting dots.

**The root namespace is the fleet file name.** `pirate init` asks for the
project name and uses it for the file and the namespace, so there is one source
of truth.

**Same-folder declarations need no import.** Everything else does. Parent and
sibling namespaces are not visible without one.

**Every type is nullable with `T?`.** One rule for scalars, classes and arrays,
with `null`, comparison and flow narrowing in the first slice. The other
operators come later.

**No `static` in the first slice.** Shared functions and constants go in a
module, so `self` never has to say whether it means the class or an instance
of it.

**Modules hold functions and constants only.** No mutable state is shared
between files.

## Open items

- A dependency's key in `dependencies` must equal its `.fleet` file name. The
  linker should check that.
- Public by default means every non-`private` member is visible to any
  importer. There is no namespace-internal level yet.
- Finer access control: independent read and write restriction, `protected`
  (needs `extends`), and accessor blocks.
- Nullable operators: `?.`, `??` and `x!`, and whether narrowing should ever
  reach fields.
- How a nullable `int` is represented in the VM's value type. Decide before the
  opcodes are written.
- Interfaces (`.ipirate`), `extends` and `implements`, then enums with
  exhaustive `match`.
- Generics. Pirate has none, so they are out of scope here.
- The module linker is a prerequisite. `import module` is currently rejected
  with SEM-013, and every feature here depends on it. FLAT's Phase 3 *is* that
  milestone — it must not first be built under the old `export` model this
  document removes.
- Import cycles between class files whose field initializers instantiate each
  other (`field B b = new B();` in both `A` and `B`) pass type registration
  and loop at run time. No compile-time check in the first slice; a later one
  would flag initializer-`new` cycles, not all cycles.
- Removing `export` is a breaking change for `docs/examples/4 - Multi Module`
  and `5 - External Module`, which would need updating.
- **Windows extension matching.** `Directory.GetFiles("*.pir")` also matches
  longer extensions that start with `pir` (`.pirate`), because .NET documents
  that a three-character extension pattern matches longer extensions. File
  discovery must compare the exact extension after enumerating, or `.pir` and
  `.pirate` will be picked up twice.
