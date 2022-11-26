using PirateLexer.Enums;

namespace PirateLexer.Interfaces;

public interface IKeyWordService
{
    TokenType GetTokenControlKeyword(string idString);
    TokenType GetTypeKeyword(string idString);
}
