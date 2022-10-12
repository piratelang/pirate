using Lexer.Tokens;

namespace Parser.Parsers.Interfaces;

public interface IParserFactory
{
    ITokenParser GetParser(Token token, List<Token> tokens);
}