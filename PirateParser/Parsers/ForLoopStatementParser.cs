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

    public override ParseResult CreateNode()
    {
        INode node;
        BaseParser? parser;
        
        ParseResult? result;
        VariableDeclarationNode VariableAssign;
        ValueNode Value;

        if (!_tokens[_index].Matches(TokenType.FOR)) throw new ParserException("No For Statement was found");

        GetVariableNode(out parser, out result, out VariableAssign);

        if (!_tokens[_index += 1].Matches(TokenType.TO)) throw new ParserException("No To Statement was found");


        GetValueNode(out parser, out result, out Value);

        if (!_tokens[_index += 1].Matches(TokenType.LEFTCURLYBRACE)) throw new ParserException("No Left Curly Braces was found");

        List<INode> Nodes = GetBodyNodes(ref parser, ref result);

        node = new ForLoopStatementNode(VariableAssign, Value, Nodes);
        return new ParseResult(node, _index);
    }

    private void GetVariableNode(out BaseParser? parser, out ParseResult? result, out VariableDeclarationNode VariableAssign)
    {
        parser = _parserFactory.GetParser(_index += 1, _tokens, Logger);
        result = parser.CreateNode();
        if (result.node is not VariableDeclarationNode) throw new ParserException("For Statement does not contain a valid variable assignment");

        VariableAssign = (VariableDeclarationNode)result.node;
        _index = result.index;
    }

    private void GetValueNode(out BaseParser parser, out ParseResult result, out ValueNode Value)
    {
        parser = _parserFactory.GetParser(_index += 1, _tokens, Logger);
        result = parser.CreateNode();
        if (result.node is not ValueNode) throw new ParserException("For Statement does not contain a valid value");

        Value = (ValueNode)result.node;
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
            if (_index + 1 >= _tokens.Count) break;
            if (_tokens[_index + 1].TokenType.Equals(TokenType.SEMICOLON))
            {
                _index++;
            }
        }

        return Nodes;
    }
}