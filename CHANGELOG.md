# Changelog for PirateLang

Ordered by Release/Milestone, then pullrequest. Listing the changes made.

# 1.1.0

### ([#71](https://github.com/joerivanarkel/PirateLang/pull/71)) XML docs for filehandler usage

- Added XML documentation for `FileReadHandler` and `FileWriteHandler`
- Removed not referenced objects

## 1.0.0

### ([#39](https://github.com/joerivanarkel/PirateLang/pull/39)) New Lexer, Parser and Interpreter

- Completely remade Parser
  - Using a Factory pattern get a Parser per token type. This is using the LL parser pattern.
  - Creates a Scope, which consits of a List of Nodes.
- Completely remade Interpreter
  - Using a Factory pattern get a Interpreter for each node.
  - Returns a BaseValue foreach node.
- Partially Rewrote Lexer, to simplify.
- Created a Object serializer, which takes a Scope and serializes it to binary.

### ([#49](https://github.com/joerivanarkel/PirateLang/pull/49)) Semicolons

- Added Semicolon token to parser.

### ([#51](https://github.com/joerivanarkel/PirateLang/pull/51)) Error Handling and Validation

- Created Custom exceptions for Type Conversion and Parsing Exceptions
- Added validation for type conversions
- Replaced null return with errors
- Created base class constructors

### ([#53](https://github.com/joerivanarkel/PirateLang/pull/53)) Fixing Float and Chaarcter issues

- Moved Logger and Factory to BaseInterpeter
- Moved Logger and Value to BaseVaule
- Added more possible nodes to variables
- Started on Validation methods per entity. #60
- Fixed Float and Character lexing errors

### ([#62](https://github.com/joerivanarkel/PirateLang/pull/62)) Command Rework

- Refined and Refactored commands
- Added a dependency injection container, from Application to the Parser and Lexer
- Moved constant variable parameters to `variables.json` and added new `EnvironmentVariables` class to get these variables
- Created repl command `shell`
- Fixed lexer for multiple inputs
- Move common interfaces to `Common.Interfaces`

### ([#66](https://github.com/joerivanarkel/PirateLang/pull/66)) Implicit usings

- Removed unnecessary usings.
- Added implicit using in `Usings.g.cs`

### ([#68](https://github.com/joerivanarkel/PirateLang/pull/68)) Lexer Rewrite

- Completely rewrote lexer to remove the static methods and fields.
- Renamed Lexer, Parser and Interpreter to PirateLexer, PirateParser and PirateInterpreter

### ([#69](https://github.com/joerivanarkel/PirateLang/pull/69)) FileHandlers

- Added a two filehandlers for writing and reading

### ([#70](https://github.com/joerivanarkel/PirateLang/pull/70)) Unit Tests

- Created Unit Tests for Lexer, Parser, Interpreter projects

## 0.1.2

### ([#32](https://github.com/joerivanarkel/PirateLang/pull/32)) Shell, CLI Improvements

- Added build command to test files. Later to be joined by #35.
- Created `.log` file, added logging in the new Common project
- Checking for modificationdate in `modules.json` and setting the new data in the build command

## 0.1.1

### ([#9](https://github.com/joerivanarkel/PirateLang/pull/9)) Shell, CLI Improvements

- Added pirate init command for creating default gitignore and gitattributes
- Moved Commands + Argument logic from Program.cs to CommandRepository.cs
  - Arguments from command line are passed as string, if no argument is given string.empty is passed
- Removed capability from non argument calls, new issue was created: [#13](https://github.com/joerivanarkel/PirateLang/issues/13) Move non argument command result to --help/-h parameter

### ([#14](https://github.com/joerivanarkel/PirateLang/pull/14)) Workflow building

- Added default .NET workflow to check if the project builds.

### ([#18](https://github.com/joerivanarkel/PirateLang/pull/18)) Added new tokens

- Added list defined between `[]`
- Added `for` loop for looping through a range
- Added `foreach` loop for looping through a collection
  - New foreach and in keywords
- Added `+=` `.` and `$` operators
- Added SYNTAX.ms for current syntax

### ([#25](https://github.com/joerivanarkel/PirateLang/pull/25)) new command structure, added -h and --help arguments

- Added new Factory pattern in `ICommand.cs` and `CommandFactory.cs`
- Defined `-h` and `--help` arguments per command

### ([#26](https://github.com/joerivanarkel/PirateLang/pull/26)) added comments between: // and ;. added true and false

- Added true and false booleans
- Ignored tokens between `//` and `;` in the parser acting as comments

### ([#37](https://github.com/joerivanarkel/PirateLang/pull/37)) Testing

- Created test for Common, Parser and Lexer
- Made `logger.Log` non static
