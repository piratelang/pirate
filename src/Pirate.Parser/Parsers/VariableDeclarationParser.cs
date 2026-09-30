using Pirate.Parser.Node.Interfaces;
using Pirate.Parser.Node;
using Pirate.Parser.Parsers.Interfaces;

namespace Pirate.Parser.Parsers;

/// <summary>
/// A parser handling variable declarations.
/// Defines a Type, an Identifier and a Value.
/// </summary>
public class VariableDeclarationParser : BaseParser, ITokenParser
{
    private ParserFactory _parserFactory { get; set; }

    public VariableDeclarationParser(List<Token> tokens, int index, ILogger logger, ParserFactory parserFactory) : base(tokens, index, logger)
    {
        _parserFactory = parserFactory;
    }

    public override ParseResult CreateNode()
    {
        INode node;
        var VariableType = _tokens[_index];

        ParseResult result;
        INode IdentifierNode;
        GetIdentifierNode(out result, out IdentifierNode);

        if (IdentifierNode is not ValueNode) throw new ParserException("Variable Identifier is not a single value");

        var Operator = _tokens[_index += 1];
        if (!Operator.Matches(TokenType.EQUALS)) throw new ParserException("No Equals assign Operator was found, following the Identifier");

        INode Value;
        GetValue(out result, out Value);

        node = new VariableDeclarationNode(VariableType, (IValueNode)IdentifierNode, Value);
        return new ParseResult(node, _index);
    }

    private void GetValue(out ParseResult result, out INode Value)
    {
        var parser = _parserFactory.GetParser(_index += 1, _tokens, Logger);
        result = parser.CreateNode();
        Value = result.Node;
        _index = result.Index;
    }

    private void GetIdentifierNode(out ParseResult result, out INode IdentifierNode)
    {
        var operationParser = new OperationParser(_tokens, _index += 1, Logger);
        result = operationParser.CreateNode();
        IdentifierNode = result.Node;
        _index = result.Index;
    }
}

