using Pirate.Syntax;

namespace Pirate.Lexer;

/// <summary>
/// The lexer's output: the token stream (always ending in TokenType.Eof, and
/// always produced even when errors occur) plus any diagnostics found along
/// the way, per docs/architecture/v2-architecture.md's collect-don't-throw
/// pipeline.
/// </summary>
public sealed record LexResult(IReadOnlyList<Token> Tokens, IReadOnlyList<Diagnostic> Diagnostics);
