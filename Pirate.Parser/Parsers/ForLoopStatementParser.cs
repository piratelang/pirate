using Pirate.Parser.Node;
using Pirate.Parser.Node.Interfaces;

namespace Pirate.Parser.Parsers;

/// <summary>
/// A parser looking for a for loop statement.
/// Defines a Variable node, a Value node and a list of body nodes.
/// </summary>
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

        if (!_tokens[_index].Matches(TokenType.FOR)) throw new ParserException(new ExceptionCode(ExceptionPrefix.PARSER, "002"));

        GetVariableNode(out parser, out result, out VariableAssign);

        if (!_tokens[_index += 1].Matches(TokenType.TO)) throw new ParserException(new ExceptionCode(ExceptionPrefix.PARSER, "018"));


        GetValueNode(out parser, out result, out Value);

        if (!_tokens[_index += 1].Matches(TokenType.LEFTCURLYBRACE)) throw new ParserException(new ExceptionCode(ExceptionPrefix.PARSER, "008"));

        List<INode> Nodes = GetBodyNodes(ref parser, ref result);

        node = new ForLoopStatementNode(VariableAssign, Value, Nodes);
        return new ParseResult(node, _index);
    }

    private void GetVariableNode(out BaseParser? parser, out ParseResult? result, out VariableDeclarationNode VariableAssign)
    {
        parser = _parserFactory.GetParser(_index += 1, _tokens, Logger);
        result = parser.CreateNode();
        if (result.Node is not VariableDeclarationNode) throw new ParserException(new ExceptionCode(ExceptionPrefix.PARSER, "019"));

        VariableAssign = (VariableDeclarationNode)result.Node;
        _index = result.Index;
    }

    private void GetValueNode(out BaseParser parser, out ParseResult result, out ValueNode Value)
    {
        parser = _parserFactory.GetParser(_index += 1, _tokens, Logger);
        result = parser.CreateNode();
        if (result.Node is not ValueNode) throw new ParserException(new ExceptionCode(ExceptionPrefix.PARSER, "020"));

        Value = (ValueNode)result.Node;
        _index = result.Index;
    }

    private List<INode> GetBodyNodes(ref BaseParser parser, ref ParseResult result)
    {
        List<INode> Nodes = new List<INode>();
        while (!_tokens[_index += 1].Matches(TokenType.RIGHTCURLYBRACE))
        {
            parser = _parserFactory.GetParser(_index, _tokens, Logger);
            result = parser.CreateNode();
            Nodes.Add(result.Node);
            _index = result.Index;
            if (_index + 1 >= _tokens.Count) break;
            if (_tokens[_index + 1].TokenType.Equals(TokenType.SEMICOLON))
            {
                _index++;
            }
        }

        return Nodes;
    }
}