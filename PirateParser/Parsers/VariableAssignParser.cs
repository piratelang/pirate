using PirateParser.Parsers.Interfaces;
using PirateParser.Node.Interfaces;
using PirateParser.Node;

namespace PirateParser.Parsers;

public class VariableAssignParser : BaseParser, ITokenParser
{
    private ParserFactory _parserFactory { get; set; }
    
    public VariableAssignParser(List<Token> tokens, Token currentToken, ILogger logger, ParserFactory parserFactory) : base(tokens, currentToken, logger)
    {
        _parserFactory = parserFactory;
    }

    public override (INode node, int index) CreateNode()
    {
        INode node;

        var index = _tokens.IndexOf(_currentToken);
        var VariableType = _currentToken;

        
        var parser = _parserFactory.GetParser(_tokens[index += 1], _tokens, Logger);
        var result = parser.CreateNode();

        var IdentifierNode = result.node;
        index = result.index;

        if (IdentifierNode is not ValueNode)
        {
            Logger.Log("Variable Identifier is not a single value", this.GetType().Name, LogType.ERROR);
            throw new ParserException("Variable Identifier is not a single value");
        }

        var Operator = _tokens[index += 1];
        if (!Operator.Matches(TokenSyntax.EQUALS))
        {
            Logger.Log("No Equals assign Operator was found", this.GetType().Name, LogType.ERROR);
            throw new ParserException("No Equals assign Operator was found, following the Identifier");
        }

        parser = _parserFactory.GetParser(_tokens[index +=1], _tokens, Logger);
        result = parser.CreateNode();
        INode Value = result.node;
        index = result.index;

        node = new VariableAssignNode(VariableType, (IValueNode)IdentifierNode, (INode)Value);
        return (node, index);
    }
}