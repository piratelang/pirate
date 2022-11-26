using PirateParser.Node;
using PirateParser.Node.Interfaces;

namespace PirateParser.Parsers;

public class IdentifierParser : BaseParser
{
    private int _startindex;
    private ParserFactory _parserFactory;

    public IdentifierParser(List<Token> tokens, int index, ILogger logger, ParserFactory parserFactory) : base(tokens, index, logger)
    {
        _startindex = index;
        _parserFactory = parserFactory;
    }
    
    public override ParseResult CreateNode()
    {
        INode node;

        OperationParser? operationParser;
        ParseResult result;
        INode identifierNode;
        GetIdentifierNode(out operationParser, out result, out identifierNode);

        if (identifierNode is not ValueNode) return CreateOperationode(out operationParser);

        var identifierValueNode = (IValueNode)identifierNode;

        var Operator = _tokens[_index += 1];
        if (!Operator.Matches(TokenType.EQUALS))
        {
            if (Operator.Matches(TokenType.LEFTPARENTHESES)) return CreateFunctionCallNode(result, identifierValueNode);
            return CreateOperationode(out operationParser);
        }

        INode Value;
        GetValueNode(out result, out Value);

        node = new VariableAssignmentNode(identifierValueNode, Value);
        return new ParseResult(node, _index);
    }

    private void GetValueNode(out ParseResult result, out INode Value)
    {
        var parser = _parserFactory.GetParser(_index += 1, _tokens, Logger);
        result = parser.CreateNode();

        Value = result.node;
        _index = result.index;
    }

    private void GetIdentifierNode(out OperationParser? operationParser, out ParseResult result, out INode identifierNode)
    {
        operationParser = new OperationParser(_tokens, _index, Logger);
        result = operationParser.CreateNode();
        identifierNode = result.node;
        _index = result.index;
    }

    private ParseResult CreateOperationode(out OperationParser operationParser)
    {
        operationParser = new OperationParser(_tokens, _startindex, Logger);
        return operationParser.CreateNode();
    }

    private ParseResult CreateFunctionCallNode(ParseResult result, IValueNode identifierValueNode)
    {
        var parameterNodes = new List<INode>();
        while (!_tokens[_index += 1].Matches(TokenType.RIGHTPARENTHESES))
        {
            var valueParser = _parserFactory.GetParser(_index, _tokens, Logger);
            result = valueParser.CreateNode();

            _index = result.index;
            parameterNodes.Add(result.node);

            if (_tokens[_index+=1].Matches(TokenType.COMMA)) continue;
            if (_tokens[_index].Matches(TokenType.RIGHTPARENTHESES)) break;
        }
        return new ParseResult(new FunctionCallNode(identifierValueNode, parameterNodes), _index);
    }
}
