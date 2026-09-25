namespace Pirate.Syntax;

/// <summary>
/// Fine-grained lexical error kinds. Each value maps to exactly one
/// <c>LEX-xxx</c> error code in the CLI's <c>ErrorMapper</c>.
/// </summary>
public enum LexErrorKind
{
    UnexpectedCharacter,
    UnterminatedStringLiteral,
    UnterminatedCharLiteral,
    UnknownEscapeSequence,
    IntegerOutOfRange,
    LoneAmpersand,
    LonePipe,
}

/// <summary>
/// A lexical error reported by <c>Pirate.Lexer</c>.
/// </summary>
public sealed class LexError : CompilationError
{
    public LexErrorKind Kind { get; }

    public LexError(LexErrorKind kind, string message, SourceLocation startLocation, SourceLocation? endLocation = null)
        : base(message, startLocation, endLocation)
    {
        Kind = kind;
    }
}
