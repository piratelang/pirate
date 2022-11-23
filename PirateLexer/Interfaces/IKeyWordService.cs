using PirateLexer.Enums;

namespace PirateLexer.Interfaces;

public interface IKeyWordService
{
    TokenType GetTokenControlKeywork(string idString);
    TokenType GetTypeKeyword(string idString);
}
