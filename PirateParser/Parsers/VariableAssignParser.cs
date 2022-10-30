using PirateParser.Parsers.Interfaces;
using PirateParser.Node.Interfaces;
using PirateParser.Node;

namespace PirateParser.Parsers;

public class VariableAssignParser : ITokenParser
{
    private List<Token> _tokens;
    private Token _currentToken;
    public ParserFactory ParserFactory { get; set; }
    public ILogger Logger { get; set; }
    
    public VariableAssignParser(List<Token> tokens, Token currentToken, ILogger logger, ParserFactory parserFactory)
    {
        _tokens = tokens;
        _currentToken = currentToken;
        Logger = logger;
        ParserFactory = parserFactory;
        logger.Log("Creating Variable Assign Parser", this.GetType().Name, LogType.INFO);
    }

    public (INode node, int index) CreateNode()
    {
        INode node;

        var index = _tokens.IndexOf(_currentToken);
        var VariableType = _currentToken;

        
        var parser = ParserFactory.GetParser(_tokens[index += 1], _tokens, Logger);
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

        parser = ParserFactory.GetParser(_tokens[index +=1], _tokens, Logger);
        result = parser.CreateNode();
        INode Value = result.node;
        index = result.index;

        node = new VariableAssignNode(VariableType, (IValueNode)IdentifierNode, (INode)Value);
        return (node, index);
    }
}