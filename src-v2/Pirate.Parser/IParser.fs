namespace Pirate.Parser

open Pirate.Lexer
open Pirate.Syntax

/// <summary>
/// The parser pipeline stage: turns a lexed token stream into a
/// <see cref="ParseResult"/> for the given file kind (docs/GRAMMAR.md §3-4).
/// Only <see cref="PirateFileKind.Module"/> has a grammar today — the
/// parameter is threaded through ahead of Phase 2's class-file grammar
/// (docs/brainstorm/FLAT_PLAN.md) so this interface doesn't change shape
/// again once it lands.
/// </summary>
type IParser =
    abstract member Parse: lexResult: LexResult * fileKind: PirateFileKind -> ParseResult
