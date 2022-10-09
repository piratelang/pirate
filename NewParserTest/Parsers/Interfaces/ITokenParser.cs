using NewParserTest.Node;
using NewPirateLexer.Tokens;
using NewParserTest.Node.Interfaces;

namespace NewParserTest.Parsers.Interfaces;

public interface ITokenParser
{
    (INode node, int index) CreateNode();
}
