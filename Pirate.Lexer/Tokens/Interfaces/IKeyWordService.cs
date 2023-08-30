using Pirate.Lexer.Enums;

namespace Pirate.Lexer.Tokens.Interfaces;

/// <inheritdoc cref="KeyWordService"/>
public interface IKeyWordService
{
    TokenType GetTokenControlKeyword(string idString);
    TokenType GetTypeKeyword(string idString);
}
