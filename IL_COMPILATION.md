# IL Compilation Architecture

The Pirate language now supports direct IL (Intermediate Language) compilation using .NET's System.Reflection.Emit functionality, allowing code to compile directly to executable .NET assemblies.

## Compilation Modes

### 1. IL Compilation (Default)
Compiles Pirate source code directly to IL bytecode using System.Reflection.Emit:

```bash
pirate compile main.pirate -o ./output
```

**Output**: Generates `.dll.info` files containing assembly metadata and method information.

### 2. ANTLR G4 Grammar Support
Uses the formal G4 grammar definition for parsing (experimental):

```bash
pirate compile main.pirate -o ./output --antlr
```

**Features**:
- Formal grammar specification in `Pirate.g4`
- ANTLR-generated lexer and parser classes
- Compatible with existing AST structure

## Architecture Components

### ILCompiler
- **File**: `Pirate.Compiler/ILCompiler.cs`
- **Purpose**: Converts Pirate AST to IL bytecode
- **Technology**: System.Reflection.Emit
- **Output**: .NET assemblies (.dll files)

### AntlrParserAdapter  
- **File**: `Pirate.Compiler/AntlrParserAdapter.cs`
- **Purpose**: Integrates ANTLR-generated parser with existing compilation pipeline
- **Technology**: ANTLR 4 runtime

### G4 Grammar
- **File**: `Pirate.g4`
- **Purpose**: Formal definition of Pirate language syntax
- **Features**: Supports all language constructs including functions, variables, control flow, and expressions

## Benefits

1. **Performance**: Direct IL compilation eliminates C# transpilation overhead
2. **Interoperability**: Generated assemblies work with .NET ecosystem
3. **Formal Grammar**: G4 specification enables tooling and language analysis
4. **Extensibility**: Clean architecture supports additional compilation targets

## Example Compilation

**Input** (`simple.pirate`):
```nim
func main() : void
{
    print("Hello, World!");
}
```

**Command**:
```bash
pirate compile simple.pirate -o ./output
```

**Output** (`simple.dll.info`):
```
IL Assembly generated for simple at 07/08/2025 10:27:23
Type: simple.Program  
Methods: main, GetType, ToString, Equals, GetHashCode
```

The generated assembly contains proper IL bytecode that can be loaded and executed by the .NET runtime.