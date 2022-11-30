using PirateParser.Node;
using PirateParser.Node.Interfaces;

namespace PirateParser.Parsers;

/// <summary>
/// A parser which handles all identifiers
/// </summary>
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
        GetIdentifierNode(out identifierNode);

        if (identifierNode is not ValueNode) return CreateOperationode(out operationParser);

        var identifierValueNode = (IValueNode)identifierNode;

        var Operator = _tokens[_index += 1];
        if (!Operator.Matches(TokenType.EQUALS))
        {
            if (Operator.Matches(TokenType.LEFTPARENTHESES)) return CreateFunctionCallNode(identifierValueNode);
            if (Operator.Matches(TokenType.LEFTBRACKET)) return CreateListAccessNode(identifierValueNode);
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

        Value = result.Node;
        _index = result.Index;
    }

    private void GetIdentifierNode(out INode identifierNode)
    {
        identifierNode = new ValueNode(_tokens[_index]);
    }

    private ParseResult CreateOperationode(out OperationParser operationParser)
    {
        operationParser = new OperationParser(_tokens, _startindex, Logger, _parserFactory);
        return operationParser.CreateNode();
    }

    private ParseResult CreateFunctionCallNode(IValueNode identifierValueNode)
    {
        var parameterNodes = new List<INode>();
        while (!_tokens[_index += 1].Matches(TokenType.RIGHTPARENTHESES))
        {
            var valueParser = _parserFactory.GetParser(_index, _tokens, Logger);
            var result = valueParser.CreateNode();

            _index = result.Index;
            parameterNodes.Add(result.Node);

            if (_tokens[_index+=1].Matches(TokenType.COMMA)) continue;
            if (_tokens[_index].Matches(TokenType.RIGHTPARENTHESES)) break;
        }
        return new ParseResult(new FunctionCallNode(identifierValueNode, parameterNodes), _index);
    }
    
    private ParseResult CreateListAccessNode(IValueNode identifierValueNode)
    {
        var parser = _parserFactory.GetParser(_index += 1, _tokens, Logger);
        var result = parser.CreateNode();
        _index = result.Index;

        if (result.Node is not IValueNode indexNode) throw new ParserException("Index must be a value");
        if (!_tokens[_index += 1].Matches(TokenType.RIGHTBRACKET)) throw new ParserException("Expected a right bracket");
        
        return new ParseResult(new ListAccessNode(identifierValueNode, (IValueNode)result.Node), _index);
    }
}
