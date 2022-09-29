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
