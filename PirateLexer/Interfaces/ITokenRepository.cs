using Pirate.Lexer;
using Pirate.Lexer.Tokens;

namespace Pirate.Lexer.Interfaces;

/// <inheritdoc cref="TokenRepository"/>
public interface ITokenRepository
{
    TokenResult MakeChar(string text, int position);
    TokenResult MakeDivide(string text, int position);
    TokenResult MakeEquals(string text, int position);
    TokenResult MakeGreaterThan(string text, int position);
    TokenResult MakeIdentifier(string text, int position);
    TokenResult MakeLessThan(string text, int position);
    TokenResult MakeNotEquals(string text, int position);
    TokenResult MakeNumber(string text, int position);
    TokenResult MakePlus(string text, int position);
    TokenResult MakeString(string text, int position);
}
