using PirateParser.Node;
using PirateParser.Node.Interfaces;

namespace PirateParser.Parsers;

public class VariableAssignmentParser : BaseParser
{
    private int _startindex;
    private ParserFactory _parserFactory;

    public VariableAssignmentParser(List<Token> tokens, int index, ILogger logger, ParserFactory parserFactory) : base(tokens, index, logger)
    {
        _startindex = index;
        _parserFactory = parserFactory;
    }
    
    public override (INode node, int index) CreateNode()
    {
        INode node;

        var VariableType = _tokens[_index];
        var operationParser = new OperationParser(_tokens, _index, Logger);
        var result = operationParser.CreateNode();

        var identifierNode = result.node;
        _index = result.index;

        if (identifierNode is not ValueNode)
        {
            operationParser = new OperationParser(_tokens, _startindex, Logger);
            return operationParser.CreateNode();
        }
        var identifierValueNode = (IValueNode)identifierNode;

        var Operator = _tokens[_index += 1];
        if (!Operator.Matches(TokenSyntax.EQUALS))
        {
            if (Operator.Matches(TokenSyntax.LEFTPARENTHESES))
            {
                return CreateFunctionCallNode(result, identifierValueNode);
            }
            operationParser = new OperationParser(_tokens, _startindex, Logger);
            return operationParser.CreateNode();
        }

        var parser = _parserFactory.GetParser(_index += 1, _tokens, Logger);
        result = parser.CreateNode();

        INode Value = result.node;
        _index = result.index;

        node = new VariableAssignmentNode(identifierValueNode, Value);
        return (node, _index);
    }

    private (INode node, int index) CreateFunctionCallNode((INode node, int index) result, IValueNode identifierValueNode)
    {
        var parameterNodes = new List<INode>();
        while (!_tokens[_index += 1].Matches(TokenSyntax.RIGHTPARENTHESES))
        {
            var typeToken = _tokens[_index];
            var valueParser = _parserFactory.GetParser(_index, _tokens, Logger);
            result = valueParser.CreateNode();

            if (result.node is not ValueNode) throw new ParserException("Function Declaration does not contain a valid parameter");
            _index = result.index;
            parameterNodes.Add(new ParameterDefinitionNode(typeToken, (ValueNode)result.node));

        }
        return (new FunctionCallNode(identifierValueNode, parameterNodes), _index);
    }
}
