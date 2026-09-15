namespace Pirate.Lexer;

/// <summary>
/// Every kind of token the v2 lexer produces, matching the lexical grammar
/// in docs/GRAMMAR.md §1. Unlike v1, there is no separate TokenGroup split —
/// the parser's precedence table replaces v1's group-based dispatch.
/// </summary>
public enum TokenType
{
    // Literals
    IntLiteral,
    FloatLiteral,
    StringLiteral,
    CharLiteral,
    Identifier,

    // Type keywords
    Var,
    Int,
    Float,
    String,
    Char,
    Bool,
    Void,

    // Control keywords
    Func,
    If,
    Else,
    While,
    For,
    In,
    To,
    Return,
    Extern,

    // Literal keywords
    True,
    False,

    // Reserved, not yet implemented (GRAMMAR.md §1.3)
    Class,
    New,

    // Assignment
    Equal,

    // Arithmetic
    Plus,
    Minus,
    Star,
    Slash,
    Percent,
    Caret,

    // Comparison
    EqualEqual,
    BangEqual,
    Less,
    LessEqual,
    Greater,
    GreaterEqual,

    // Logical
    AmpAmp,
    PipePipe,
    Bang,

    // Grouping
    LeftParen,
    RightParen,
    LeftBrace,
    RightBrace,
    LeftBracket,
    RightBracket,

    // Separators
    Comma,
    Colon,
    Semicolon,
    Dot,

    Eof
}
