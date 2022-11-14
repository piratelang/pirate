using PirateParser.Node;
using PirateParser.Node.Interfaces;

namespace PirateParser.Parsers;

public class VariableAssignmentParser : BaseParser
{
    private ParserFactory _parserFactory;
    private OperationParser _operationParser;

    public VariableAssignmentParser(List<Token> tokens, Token currentToken, ILogger logger, ParserFactory parserFactory, OperationParser operationParser) : base(tokens, currentToken, logger)
    {
        _parserFactory = parserFactory;
        _operationParser = operationParser;
    }
    
    public override (INode node, int index) CreateNode()
    {
        INode node;

        var index = _tokens.IndexOf(_currentToken);
        var VariableType = _currentToken;

        var parser = _parserFactory.GetParser(_currentToken, _tokens, Logger);
        var result = parser.CreateNode();

        var identifierNode = result.node;
        index = result.index;

        if (identifierNode is not ValueNode)
        {
            Logger.Log("Variable Identifier is not a single value", this.GetType().Name, LogType.ERROR);
            throw new ParserException("Variable Identifier is not a single value");
        }
        var identifierValueNode = (IValueNode)identifierNode;

        var Operator = _tokens[index += 1];
        if (!Operator.Matches(TokenSyntax.EQUALS))
        {
            return _operationParser.CreateNode();
        }

        parser = _parserFactory.GetParser(_tokens[index += 1], _tokens, Logger);
        result = parser.CreateNode();

        INode Value = result.node;
        index = result.index;

        node = new VariableAssignmentNode(identifierValueNode, Value);
        return (node, index);
    }
}