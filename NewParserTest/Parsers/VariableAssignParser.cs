using NewPirateLexer.Tokens;
using NewParserTest.Parsers.Interfaces;
using NewParserTest.Node.Interfaces;
using NewPirateLexer.Enums;
using NewParserTest.Node;

namespace NewParserTest.Parsers;

public class VariableAssignParser : ITokenParser
{
    private List<Token> _tokens;
    private Token _currentToken;
    public VariableAssignParser(List<Token> tokens, Token currentToken)
    {
        _tokens = tokens;
        _currentToken = currentToken;
    }

    public (INode node, int index) CreateNode()
    {
        INode node = null;

        var index = _tokens.IndexOf(_currentToken);
        var VariableTyepe = _currentToken;

        var parserFactory = new ParserFactory();
        var parser = parserFactory.GetParser(_tokens[index += 1], _tokens);
        var result = parser.CreateNode();

        var IdentifierNode = result.node;
        index = result.index;

        var Operator = _tokens[index += 1];
        if (!Operator.Matches(TokenSyntax.EQUALS))
        {
            return (null, 0);
        }

        parser = parserFactory.GetParser(_tokens[index +=1], _tokens);
        result = parser.CreateNode();
        INode Value = result.node;
        index = result.index;

        if (IdentifierNode is not ValueNode)
        {
            return (null, 0);
        }

        node = new VariableAssignNode(IdentifierNode, Value);
        return (node, index);
    }
}