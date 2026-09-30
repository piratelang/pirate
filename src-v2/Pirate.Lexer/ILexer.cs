namespace Pirate.Lexer;

/// <summary>
/// The lexer pipeline stage: turns source text into a token stream, per
/// docs/GRAMMAR.md §1.
/// </summary>
public interface ILexer
{
    /// <summary>
    /// Scans <paramref name="source"/> into a <see cref="LexResult"/>. Always
    /// produces a token stream (ending in <c>TokenType.Eof</c>), even when
    /// <see cref="LexResult.Errors"/> is non-empty — collect-don't-throw, per
    /// docs/architecture/v2-architecture.md.
    /// </summary>
    LexResult Tokenize(string source);
}
