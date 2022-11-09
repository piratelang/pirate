using PirateLexer.Enums;

namespace PirateLexer.Interfaces;

public interface IKeyWordService
{
    TokenControlKeyword GetTokenControlKeywork(string idString);
    TokenTypeKeyword GetTypeKeyword(string idString);
}
