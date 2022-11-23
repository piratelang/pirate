using PirateParser.Node;
using PirateParser.Node.Interfaces;

namespace PirateParser.Parsers;

public class ForLoopStatementParser : BaseParser
{
    private ParserFactory _parserFactory { get; set; }

    public ForLoopStatementParser(List<Token> tokens, int index, ILogger logger, ParserFactory parserFactory) : base(tokens, index, logger)
    {
        _parserFactory = parserFactory;
    }

    public override (INode node, int index) CreateNode()
    {
        INode node;

        var _currentToken = _tokens[_index];

        if (!_currentToken.Matches(TokenType.FOR)) throw new ParserException("No For Statement was found");
        

        var parser = _parserFactory.GetParser(_index += 1, _tokens, Logger);
        var result = parser.CreateNode();
        if (result.node is not VariableDeclarationNode) throw new ParserException("For Statement does not contain a valid variable assignment");
        

        VariableDeclarationNode VariableAssign = (VariableDeclarationNode)result.node;
        _index = result.index;

        if (!_tokens[_index += 1].Matches(TokenType.TO)) throw new ParserException("No To Statement was found");
        

        parser = _parserFactory.GetParser(_index += 1, _tokens, Logger);
        result = parser.CreateNode();
        if (result.node is not ValueNode) throw new ParserException("For Statement does not contain a valid value");
        

        var Value = (ValueNode)result.node;
        _index = result.index;

        if (!_tokens[_index += 1].Matches(TokenType.LEFTCURLYBRACE)) throw new ParserException("No Left Curly Braces was found");
        

        List<INode> Nodes = new List<INode>();
        while (!_tokens[_index += 1].Matches(TokenType.RIGHTCURLYBRACE))
        {
            parser = _parserFactory.GetParser(_index, _tokens, Logger);
            result = parser.CreateNode();
            Nodes.Add(result.node);
            _index = result.index;
            if (_index + 1>= _tokens.Count) break;
            if (_tokens[_index+1].TokenType.Equals(TokenType.SEMICOLON))
            {
                _index++;
            }
        }

        node = new ForLoopStatementNode(VariableAssign, Value, Nodes);
        return (node, _index);
    }
}