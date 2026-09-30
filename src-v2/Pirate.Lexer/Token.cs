using Pirate.Syntax;

namespace Pirate.Lexer;

/// <summary>
/// A single lexed token. <see cref="Lexeme"/> is the token's spelling
/// (identifier text, decoded string/char contents, or the operator's
/// symbol); <see cref="Value"/> is the parsed literal value for
/// IntLiteral/FloatLiteral/StringLiteral/CharLiteral/True/False, and null
/// for everything else. A value type (not a class like v1's BaseValue
/// hierarchy) so tokenizing a file never heap-allocates per token.
/// </summary>
public readonly record struct Token(TokenType Type, string Lexeme, object? Value, SourceLocation Location);
