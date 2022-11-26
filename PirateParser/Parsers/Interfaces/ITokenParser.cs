using PirateParser.Node.Interfaces;

namespace PirateParser.Parsers.Interfaces;

public interface ITokenParser
{
    ParseResult CreateNode();
}
