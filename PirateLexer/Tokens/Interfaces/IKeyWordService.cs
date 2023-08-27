using Pirate.Lexer.Enums;
using Pirate.Lexer.Tokens;

namespace Pirate.Lexer.Tokens.Interfaces;

/// <inheritdoc cref="KeyWordService"/>
public interface IKeyWordService
{
    TokenType GetTokenControlKeyword(string idString);
    TokenType GetTypeKeyword(string idString);
}
