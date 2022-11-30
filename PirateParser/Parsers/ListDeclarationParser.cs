using PirateParser.Node;
using PirateParser.Node.Interfaces;

namespace PirateParser.Parsers;

/// <summary>
/// A 
public class ListDeclarationParser : BaseParser
{
    private ParserFactory _parserFactory { get; set; }

    public ListDeclarationParser(List<Token> tokens, int index, ILogger logger, ParserFactory parserFactory) : base(tokens, index, logger)
    {
        _parserFactory = parserFactory;
    }

    public override ParseResult CreateNode()
    {
        INode node;

        if (!_tokens[_index].Matches(TokenType.LEFTBRACKET)) throw new ParserException("No Left Square Bracket was found");
        List<INode> Nodes = GetBodyNodes();

        node = new ListDeclarationNode(Nodes);
        return new ParseResult(node, _index);
    }

    private List<INode> GetBodyNodes()
    {
        List<INode> Nodes = new();
        _index++;
        while (!_tokens[_index].Matches(TokenType.RIGHTBRACKET))
        {
            var parser = _parserFactory.GetParser(_index, _tokens, Logger);
            var result = parser.CreateNode();
            Nodes.Add(result.Node);
            _index = result.Index;
            if (_tokens[_index+1].TokenType.Equals(TokenType.COMMA))
            {
                _index += 1;
            }
            _index++;
        }

        return Nodes;
    }
}