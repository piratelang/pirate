using Pirate.Common.Interfaces;
using Pirate.Lexer.Enums;
using Pirate.Lexer.Tokens;
using Pirate.Parser;
using Pirate.Parser.Node.Interfaces;
using Pirate.Parser.Node;

namespace Pirate.Parser.Parsers;

/// <summary>
/// A parser which parses a while loop statement.
/// Defines a comaprison operation and a body.
/// </summary>
public class WhileLoopStatementParser : BaseParser
{
    private ParserFactory _parserFactory { get; set; }

    public WhileLoopStatementParser(List<Token> tokens, int index, ILogger logger, ParserFactory parserFactory) : base(tokens, index, logger)
    {
        _parserFactory = parserFactory;
    }

    public override ParseResult CreateNode()
    {
        INode node;

        if (!_tokens[_index].Matches(TokenType.WHILE)) throw new ParserException("No While Statement was found");

        BaseParser parser;
        ParseResult result;
        IOperationNode Operation;

        GetOperationNode(out parser, out result, out Operation);

        if (!_tokens[_index += 1].Matches(TokenType.LEFTCURLYBRACE)) throw new ParserException("No Left Curly Braces was found");

        List<INode> Nodes = GetBodyNodes(ref parser, ref result);

        node = new WhileLoopStatementNode(Operation, Nodes);
        return new ParseResult(node, _index);
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
            if (_tokens[_index++].TokenType.Equals(TokenType.SEMICOLON))
            {
                _index++;
            }
        }

        return Nodes;
    }

    private void GetOperationNode(out BaseParser parser, out ParseResult result, out IOperationNode Operation)
    {
        parser = _parserFactory.GetParser(_index += 1, _tokens, Logger);
        result = parser.CreateNode();
        if (result.Node is not IOperationNode) throw new ParserException("While Statement does not contain a valid operation");

        Operation = (IOperationNode)result.Node;
        _index = result.Index;
    }
}