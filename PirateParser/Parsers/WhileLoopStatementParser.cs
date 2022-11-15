using PirateParser.Node;
using PirateParser.Node.Interfaces;

namespace PirateParser.Parsers;

public class WhileLoopStatementParser : BaseParser
{
    private ParserFactory _parserFactory { get; set; }

    public WhileLoopStatementParser(List<Token> tokens, int index, ILogger logger, ParserFactory parserFactory) : base(tokens, index, logger)
    {
        _parserFactory = parserFactory;
    }

    public override (INode node, int index) CreateNode()
    {
        INode node;

        if (!_tokens[_index].Matches(TokenControlKeyword.WHILE))
        {
            Logger.Log("No While Statement was found", this.GetType().Name, LogType.ERROR);
            throw new ParserException("No While Statement was found");
        }

        var parser = _parserFactory.GetParser(_index += 1, _tokens, Logger);
        var result = parser.CreateNode();
        if (result.node is not IOperationNode)
        {
            Logger.Log("While Statement does not contain a valid operation", this.GetType().Name, LogType.ERROR);
            throw new ParserException("While Statement does not contain a valid operation");
        }

        IOperationNode Operation = (IOperationNode)result.node;
        _index = result.index;

        if (!_tokens[_index += 1].Matches(TokenSyntax.LEFTCURLYBRACE))
        {
            Logger.Log("No Left Curly Brace was found", this.GetType().Name, LogType.ERROR);
            throw new ParserException("No Left Curly Braces was found");
        }

        List<INode> Nodes = new List<INode>();
        while (!_tokens[_index += 1].Matches(TokenSyntax.RIGHTCURLYBRACE))
        {
            parser = _parserFactory.GetParser(_index, _tokens, Logger);
            result = parser.CreateNode();
            Nodes.Add(result.node);
            _index = result.index;
            if (_tokens[_index++].TokenType.Equals(TokenSyntax.SEMICOLON))
            {
                _index++;
            }
        }

        node = new WhileLoopStatementNode(Operation, Nodes);
        return (node, _index);

    }
}