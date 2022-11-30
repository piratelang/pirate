using PirateParser.Node;
using PirateParser.Node.Interfaces;

namespace PirateParser.Parsers;

/// <summary>
/// A parser looking for a foreach loop statement.
/// Defines a Variable node, a Value node and a list of body nodes.
/// </summary>
public class ForeachLoopStatementParser : BaseParser
{
    private ParserFactory _parserFactory { get; set; }

    public ForeachLoopStatementParser(List<Token> tokens, int index, ILogger logger, ParserFactory parserFactory) : base(tokens, index, logger)
    {
        _parserFactory = parserFactory;
    }

    public override ParseResult CreateNode()
    {
        INode node;

        if (!_tokens[_index].Matches(TokenType.FOREACH)) throw new ParserException("No Foreach Statement was found");

        var VariableAssign = GetVariableNode();

        if (!_tokens[_index += 1].Matches(TokenType.IN)) throw new ParserException("No In Statement was found");

        var Value = GetValueNode();

        if (!_tokens[_index += 1].Matches(TokenType.LEFTCURLYBRACE)) throw new ParserException("No Left Curly Braces was found");

        var Nodes = GetBodyNodes();

        node = new ForeachLoopStatementNode(VariableAssign, Value, Nodes);
        return new ParseResult(node, _index);
    }

    private ParameterDefinitionNode GetVariableNode()
    {
        var typeToken = _tokens[_index += 1];
        var parser = _parserFactory.GetParser(_index += 1, _tokens, Logger);
        var result = parser.CreateNode();

        if (result.Node is not ValueNode) throw new ParserException("Foreach Statement does not contain a valid variable assignment");

        var variable = new ParameterDefinitionNode(typeToken, (ValueNode)result.Node);
        return variable;
    }

    private ValueNode GetValueNode()
    {
        var parser = _parserFactory.GetParser(_index += 1, _tokens, Logger);
        var result = parser.CreateNode();
        if (result.Node is not ValueNode) throw new ParserException("Foreach Statement does not contain a valid value");

        _index = result.Index;
        return (ValueNode)result.Node;
    }

    private List<INode> GetBodyNodes()
    {
        var Nodes = new List<INode>();
        while (!_tokens[_index+=1].Matches(TokenType.RIGHTCURLYBRACE))
        {
            var parser = _parserFactory.GetParser(_index, _tokens, Logger);
            var result = parser.CreateNode();
            Nodes.Add(result.Node);
            _index = result.Index;

            if (_tokens[_index+1].Matches(TokenType.SEMICOLON)) _index += 1;
            if (_index >= _tokens.Count) throw new ParserException("Foreach Statement does not contain a valid body");
            
        }

        return Nodes;
    }
}
