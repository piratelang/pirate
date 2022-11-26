using PirateParser.Node;
using PirateParser.Node.Interfaces;

namespace PirateParser.Parsers;

public class IfStatementParser : BaseParser
{
    private ParserFactory _parserFactory { get; set; }

    public IfStatementParser(List<Token> tokens, int index, ILogger logger, ParserFactory parserFactory) : base(tokens, index, logger)
    {
        _parserFactory = parserFactory;
    }

    public override ParseResult CreateNode()
    {
        INode node;
        BaseParser parser;
        ParseResult result;
        IOperationNode Operation;

        if (!_tokens[_index].Matches(TokenType.IF)) throw new ParserException("No If Statement was found");

        GetOperationNode(out parser, out result, out Operation);

        if (!_tokens[_index += 1].Matches(TokenType.LEFTCURLYBRACE)) throw new ParserException("No Left Curly Braces was found");

        List<INode> Nodes = GetBodyNodes(ref parser, ref result);

        if (_index + 1 == _tokens.Count) return new ParseResult(new IfStatementNode(Operation, Nodes), _index);
        if (!_tokens[_index + 1].Matches(TokenType.ELSE)) return new ParseResult(new IfStatementNode(Operation, Nodes), _index);
        if (!_tokens[_index += 2].Matches(TokenType.LEFTCURLYBRACE)) throw new ParserException("No Left Curly Braces was found");
        
        List<INode> ElseNodes = GetElseBodyNodes(ref parser, ref result);
        node = new IfStatementNode(Operation, Nodes, ElseNodes);
        return new ParseResult(node, _index);
    }

    private List<INode> GetElseBodyNodes(ref BaseParser parser, ref ParseResult result)
    {
        List<INode> ElseNodes = new List<INode>();
        while (!_tokens[_index += 1].Matches(TokenType.RIGHTCURLYBRACE))
        {
            parser = _parserFactory.GetParser(_index, _tokens, Logger);
            result = parser.CreateNode();
            ElseNodes.Add(result.node);
            _index = result.index;
            if (_tokens[_index++].TokenType.Equals(TokenType.SEMICOLON))
            {
                _index++;
            }
        }

        return ElseNodes;
    }

    private void GetOperationNode(out BaseParser parser, out ParseResult result, out IOperationNode Operation)
    {
        parser = _parserFactory.GetParser(_index += 1, _tokens, Logger);
        result = parser.CreateNode();
        if (result.node is not IOperationNode) throw new ParserException("If Statement does not contain a valid operation");

        Operation = (IOperationNode)result.node;
        _index = result.index;
    }

    private List<INode> GetBodyNodes(ref BaseParser parser, ref ParseResult result)
    {
        List<INode> Nodes = new List<INode>();
        while (!_tokens[_index += 1].Matches(TokenType.RIGHTCURLYBRACE))
        {
            parser = _parserFactory.GetParser(_index, _tokens, Logger);
            result = parser.CreateNode();
            Nodes.Add(result.node);
            _index = result.index;
            if (_tokens[_index++].TokenType.Equals(TokenType.SEMICOLON))
            {
                _index++;
            }
        }

        return Nodes;
    }
}
