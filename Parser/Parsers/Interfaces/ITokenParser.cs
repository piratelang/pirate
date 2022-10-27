using Parser.Node.Interfaces;

namespace Parser.Parsers.Interfaces;

public interface ITokenParser
{
    (INode node, int index) CreateNode();
}
