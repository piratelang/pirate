namespace PirateParser.Parsers.Interfaces;

public interface IParserFactory
{
    BaseParser GetParser(Token token, List<Token> tokens, ILogger logger);
}