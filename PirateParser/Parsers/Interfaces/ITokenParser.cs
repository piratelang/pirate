using PirateParser.Node.Interfaces;

namespace PirateParser.Parsers.Interfaces;

public interface ITokenParser
{
    (INode node, int index) CreateNode();
}
