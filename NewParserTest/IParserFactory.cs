using NewPirateLexer.Tokens;

namespace NewParserTest;

public interface IParserFactory
{
    ITokenParser GetParser(Token token, List<Token> tokens);
}