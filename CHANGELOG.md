# Changelog for PirateLang
Ordered by Release/Milestone, then pullrequest. Listing the changes made.

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
