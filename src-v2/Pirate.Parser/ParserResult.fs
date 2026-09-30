namespace Pirate.Parser

open System.Collections.Generic
open Pirate.Lexer
open Pirate.Syntax
open Pirate.Syntax.Nodes

/// <summary>
/// Shared mutable parse cursor threaded through both the Pratt expression
/// parser (<c>Pratt.fs</c>) and the statement/top-level parser (<c>Parser.fs</c>).
/// A single record holds the token stream, the current position, and the
/// accumulated errors; both modules mutate <c>Pos</c>/<c>Errors</c> in place
/// so expression and statement parsing always see one consistent cursor.
/// </summary>
type internal State =
    { Tokens: IReadOnlyList<Token>
      mutable Pos: int
      Errors: ResizeArray<CompilationError> }

/// <summary>
/// The parser's output: a <see cref="ProgramNode"/> (always produced, even
/// when errors occur — potentially partial) plus any <see cref="SyntaxError"/>
/// found along the way (lexer errors are *not* merged in here — they stay on
/// <c>LexResult.Errors</c> and the CLI concatenates both lists).
/// Matches the lexer's <c>LexResult</c> shape for a uniform
/// collect-don't-throw pipeline.
/// </summary>
type ParseResult(program: ProgramNode option, errors: IReadOnlyList<CompilationError>) =
    member _.Program = program
    member _.Errors = errors