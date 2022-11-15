using PirateParser.Node;
using PirateParser.Node.Interfaces;

namespace PirateParser.Parsers;

public class VariableAssignmentParser : BaseParser
{
    private ParserFactory _parserFactory;

    public VariableAssignmentParser(List<Token> tokens, int index, ILogger logger, ParserFactory parserFactory) : base(tokens, index, logger)
    {
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
            Logger.Log("Variable Identifier is not a single value", this.GetType().Name, LogType.ERROR);
            throw new ParserException("Variable Identifier is not a single value");
        }
        var identifierValueNode = (IValueNode)identifierNode;

        var Operator = _tokens[_index += 1];
        if (!Operator.Matches(TokenSyntax.EQUALS))
        {
            operationParser = new OperationParser(_tokens, _index, Logger);
            return operationParser.CreateNode();
        }

        var parser = _parserFactory.GetParser(_index += 1, _tokens, Logger);
        result = parser.CreateNode();

        INode Value = result.node;
        _index = result.index;

        node = new VariableAssignmentNode(identifierValueNode, Value);
        return (node, _index);
    }
}