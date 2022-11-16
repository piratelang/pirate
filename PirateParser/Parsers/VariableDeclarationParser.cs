using PirateParser.Parsers.Interfaces;
using PirateParser.Node.Interfaces;
using PirateParser.Node;

namespace PirateParser.Parsers;

public class VariableDeclarationParser : BaseParser, ITokenParser
{
    private ParserFactory _parserFactory { get; set; }
    
    public VariableDeclarationParser(List<Token> tokens, int index, ILogger logger, ParserFactory parserFactory) : base(tokens, index, logger)
    {
        _parserFactory = parserFactory;
    }

    public override (INode node, int index) CreateNode()
    {
        INode node;
        var VariableType = _tokens[_index];

        var operationParser = new OperationParser(_tokens, _index+=1, Logger);
        var result = operationParser.CreateNode();

        var IdentifierNode = result.node;
        _index = result.index;

        if (IdentifierNode is not ValueNode)
        {
            Logger.Log("Variable Identifier is not a single value", this.GetType().Name, LogType.ERROR);
            throw new ParserException("Variable Identifier is not a single value");
        }

        var Operator = _tokens[_index += 1];
        if (!Operator.Matches(TokenSyntax.EQUALS))
        {
            Logger.Log("No Equals assign Operator was found", this.GetType().Name, LogType.ERROR);
            throw new ParserException("No Equals assign Operator was found, following the Identifier");
        }

        var parser = _parserFactory.GetParser(_index +=1, _tokens, Logger);
        result = parser.CreateNode();
        INode Value = result.node;
        _index = result.index;

        node = new VariableDeclarationNode(VariableType, (IValueNode)IdentifierNode, (INode)Value);
        return (node, _index);
    }
}

