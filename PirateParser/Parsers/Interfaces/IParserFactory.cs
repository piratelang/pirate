namespace PirateParser.Parsers.Interfaces;

public interface IParserFactory
{
    BaseParser GetParser(int index, List<Token> tokens, ILogger logger);
}