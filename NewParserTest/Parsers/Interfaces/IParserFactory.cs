using NewPirateLexer.Tokens;

namespace NewParserTest.Parsers.Interfaces;

public interface IParserFactory
{
    ITokenParser GetParser(Token token, List<Token> tokens);
}