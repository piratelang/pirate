using PirateLexer.Enums;

namespace PirateLexer.Tokens.Interfaces;

/// <inheritdoc cref="KeyWordService"/>
public interface IKeyWordService
{
    TokenType GetTokenControlKeyword(string idString);
    TokenType GetTypeKeyword(string idString);
}
