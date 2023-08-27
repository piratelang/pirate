using Pirate.Common.Interfaces;
using Pirate.Lexer.Tokens;

namespace Pirate.Parser.Parsers.Interfaces;

/// <inheritdoc cref="ParserFactory"/>
public interface IParserFactory
{
    BaseParser GetParser(int index, List<Token> tokens, ILogger logger);
}