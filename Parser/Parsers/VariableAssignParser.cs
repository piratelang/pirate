using Lexer.Tokens;
using Parser.Parsers.Interfaces;
using Parser.Node.Interfaces;
using Lexer.Enums;
using Parser.Node;
using Common;
using Common.Enum;

namespace Parser.Parsers;

public class VariableAssignParser : ITokenParser
{
    private List<Token> _tokens;
    private Token _currentToken;
    public Logger Logger { get; set; }
    public VariableAssignParser(List<Token> tokens, Token currentToken, Logger logger)
    {
        _tokens = tokens;
        _currentToken = currentToken;
        Logger = logger;
        logger.Log("Creating Variable Assign Parser", this.GetType().Name, LogType.INFO);
    }

    public (INode node, int index) CreateNode()
    {
        INode node = null;

        var index = _tokens.IndexOf(_currentToken);
        var VariableType = _currentToken;

        var parserFactory = new ParserFactory();
        var parser = parserFactory.GetParser(_tokens[index += 1], _tokens, Logger);
        var result = parser.CreateNode();

        var IdentifierNode = result.node;
        index = result.index;

        var Operator = _tokens[index += 1];
        if (!Operator.Matches(TokenSyntax.EQUALS))
        {
            Logger.Log("No Equals assign Operator was found", this.GetType().Name, LogType.ERROR);
            return (null, 0);
        }

        parser = parserFactory.GetParser(_tokens[index +=1], _tokens, Logger);
        result = parser.CreateNode();
        INode Value = result.node;
        index = result.index;

        if (IdentifierNode is not ValueNode)
        {
            Logger.Log("Variable Identifier is not a single value", this.GetType().Name, LogType.ERROR);
            return (null, 0);
        }

        node = new VariableAssignNode(VariableType, (IValueNode)IdentifierNode, (IValueNode)Value);
        return (node, index);
    }
}