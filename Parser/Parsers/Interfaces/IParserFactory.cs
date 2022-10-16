using Lexer.Tokens;
using Common;

namespace Parser.Parsers.Interfaces;

public interface IParserFactory
{
    ITokenParser GetParser(Token token, List<Token> tokens, ILogger logger);
}