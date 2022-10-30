using PirateLexer.Enums;

namespace PirateLexer.Tokens;

public interface IKeyWorkService
{
    TokenControlKeyword GetTokenControlKeywork(string idString);
    TokenTypeKeyword GetTypeKeyword(string idString);
}
