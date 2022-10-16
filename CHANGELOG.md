# Changelog for PirateLang
Ordered by Release/Milestone, then pullrequest. Listing the changes made.

## 1.0.0
### ([#39](https://github.com/joerivanarkel/PirateLang/pull/32)) New Lexer, Parser and Interpreter<br>
- Completely remade Parser
    - Using a Factory pattern get a Parser per token type. This is using the LL parser pattern.
    - Creates a Scope, which consits of a List of Nodes.
- Completely remade Interpreter
    - Using a Factory pattern get a Interpreter for each node.
    - Returns a BaseValue foreach node.
- Partially Rewrote Lexer, to simplify.
- Created a Object serializer, which takes a Scope and serializes it to binary.

## 0.1.2
### ([#32](https://github.com/joerivanarkel/PirateLang/pull/32)) Shell, CLI Improvements<br>
- Added build command to test files. Later to be joined by #35.
- Created `.log` file, added logging in the new Common project
- Checking for modificationdate in `modules.json` and setting the new data in the build command

## 0.1.1
### ([#9](https://github.com/joerivanarkel/PirateLang/pull/9)) Shell, CLI Improvements<br>
- Added pirate init command for creating default gitignore and gitattributes
- Moved Commands + Argument logic from Program.cs to CommandRepository.cs
  - Arguments from command line are passed as string, if no argument is given string.empty is passed
- Removed capability from non argument calls, new issue was created: [#13](https://github.com/joerivanarkel/PirateLang/issues/13) Move non argument command result to --help/-h parameter 

### ([#14](https://github.com/joerivanarkel/PirateLang/pull/14)) Workflow building<br>
- Added default .NET workflow to check if the project builds.

### ([#18](https://github.com/joerivanarkel/PirateLang/pull/18)) Added new tokens<br>
- Added list defined between `[]`
- Added `for` loop for looping through a range
- Added `foreach` loop for looping through a collection
    - New foreach and in keywords
- Added `+=` `.` and `$` operators
- Added SYNTAX.ms for current syntax

### ([#25](https://github.com/joerivanarkel/PirateLang/pull/25)) new command structure, added -h and --help arguments<br>
- Added new Factory pattern in `ICommand.cs` and `CommandFactory.cs`
- Defined `-h` and `--help` arguments per command

### ([#26](https://github.com/joerivanarkel/PirateLang/pull/26)) added comments between: // and ;. added true and false<br>
- Added true and false booleans
- Ignored tokens between `//` and `;` in the parser acting as comments


### ([#37](https://github.com/joerivanarkel/PirateLang/pull/37)) Testing<br>
- Created test for Common, Parser and Lexer
- Made `logger.Log` non static
