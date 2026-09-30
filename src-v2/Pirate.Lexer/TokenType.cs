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
    Const,
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

    // Module-system keywords (GRAMMAR.md §1.3; soft keywords 'standard',
    // 'module', 'external' stay ordinary identifiers)
    Import,

    // Visibility keyword (replaces 'export' — GRAMMAR.md §3.3)
    Private,

    // Class keywords (GRAMMAR.md §4)
    Field,
    Constructor,
    Readonly,
    Self,
    Null,

    // Literal keywords
    True,
    False,

    // 'new' is an expression keyword (GRAMMAR.md §3.6/§4)
    New,

    // Reserved, not yet implemented (GRAMMAR.md §1.3, §4/§5)
    Class,
    Super,
    Extends,
    Implements,
    Abstract,
    Override,
    Static,

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
    Question,

    Eof
}
