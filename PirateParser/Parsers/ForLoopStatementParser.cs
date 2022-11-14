using PirateParser.Node;
using PirateParser.Node.Interfaces;

namespace PirateParser.Parsers;

public class ForLoopStatementParser : BaseParser
{
    private ParserFactory _parserFactory { get; set; }

    public ForLoopStatementParser(List<Token> tokens, Token token, ILogger logger, ParserFactory parserFactory) : base(tokens, token, logger)
    {
        _parserFactory = parserFactory;
    }

    public override (INode node, int index) CreateNode()
    {
        INode node;

        var index = _tokens.IndexOf(_currentToken);

        if (!_currentToken.Matches(TokenControlKeyword.FOR))
        {
            Logger.Log("No For Statement was found", this.GetType().Name, LogType.ERROR);
            throw new ParserException("No For Statement was found");
        }

        var parser = _parserFactory.GetParser(_tokens[index += 1], _tokens, Logger);
        var result = parser.CreateNode();
        if (result.node is not VariableDeclarationNode)
        {
            Logger.Log("For Statement does not contain a valid variable assignment", this.GetType().Name, LogType.ERROR);
            throw new ParserException("For Statement does not contain a valid variable assignment");
        }

        VariableDeclarationNode VariableAssign = (VariableDeclarationNode)result.node;
        index = result.index;

        if (!_tokens[index += 1].Matches(TokenControlKeyword.TO))
        {
            Logger.Log("No To Statement was found", this.GetType().Name, LogType.ERROR);
            throw new ParserException("No To Statement was found");
        }

        parser = _parserFactory.GetParser(_tokens[index += 1], _tokens, Logger);
        result = parser.CreateNode();
        if (result.node is not ValueNode)
        {
            Logger.Log("For Statement does not contain a valid value", this.GetType().Name, LogType.ERROR);
            throw new ParserException("For Statement does not contain a valid value");
        }

        ValueNode Value = (ValueNode)result.node;
        index = result.index;

        if (!_tokens[index += 1].Matches(TokenSyntax.LEFTCURLYBRACE))
        {
            Logger.Log("No Left Curly Brace was found", this.GetType().Name, LogType.ERROR);
            throw new ParserException("No Left Curly Braces was found");
        }

        List<INode> Nodes = new List<INode>();
        while (!_tokens[index += 1].Matches(TokenSyntax.RIGHTCURLYBRACE))
        {
            parser = _parserFactory.GetParser(_tokens[index], _tokens, Logger);
            result = parser.CreateNode();
            Nodes.Add(result.node);
            index = result.index;
            if (_tokens[index+1].TokenType.Equals(TokenSyntax.SEMICOLON))
            {
                index++;
            }
            if (index >= _tokens.Count) break;
        }

        node = new ForLoopStatementNode(VariableAssign, Value, Nodes);
        return (node, index);
    }
}