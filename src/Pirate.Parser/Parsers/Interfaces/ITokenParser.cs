namespace Pirate.Parser.Parsers.Interfaces;

/// <summary>
/// Interface for all token parsers.
/// </summary>
public interface ITokenParser
{
    ParseResult CreateNode();
}
