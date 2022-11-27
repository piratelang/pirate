namespace PirateParser.Parsers.Interfaces;

/// <inheritdoc cref="ParserFactory"/>
public interface IParserFactory
{
    BaseParser GetParser(int index, List<Token> tokens, ILogger logger);
}