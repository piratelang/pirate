using PirateParser.Node;
using PirateParser.Parsers.Interfaces;
using PirateParser.Node.Interfaces;

namespace PirateParser.Parsers;

/// <summary>
/// A parser looking for either a Binary operation or a Comparison operation.
/// Defines a Left and Right node, and an Operator.
/// </summary>
public class OperationParser : BaseParser, ITokenParser
{
    private ParserFactory _parserFactory { get; set; }
    public OperationParser(List<Token> tokens, int index, ILogger logger, ParserFactory parserFactory) : base(tokens, index, logger) 
    {
        _parserFactory = parserFactory;
    }

    public override ParseResult CreateNode()
    {
        INode node;
        INode LeftNode;
        if(_tokens[_index+1].TokenType.Equals(TokenType.LEFTBRACKET) || _tokens[_index+1].TokenType.Equals(TokenType.LEFTPARENTHESES))
        {
            var parser = _parserFactory.GetParser(_index, _tokens, Logger);
            var result = parser.CreateNode();
            LeftNode = result.Node;
        }
        else
        {
            LeftNode = new ValueNode(_tokens[_index]);
        }

        if (_tokens.Count == _index + 1) return ReturnValueNode(LeftNode);

        var OperatorToken = _tokens[_index + 1];
        if (OperatorToken.TokenGroup != TokenGroup.OPERATORS && OperatorToken.TokenGroup != TokenGroup.COMPARISONOPERATORS) return ReturnValueNode(LeftNode);

        node = ExploreBinaryOperation(ref LeftNode);

        if (_tokens.Count == _index + 1)
        {
            Logger.Log("Returning Binary Operation Node", LogType.INFO);
            return new ParseResult(node, _index);
        }

        OperatorToken = _tokens[_index + 1];
        if (OperatorToken.TokenGroup == TokenGroup.COMPARISONOPERATORS)
        {
            CreateComparisonOperation(out node, LeftNode, out OperatorToken);
        }

        Logger.Log("Returning Comparison or Binary Operation Node", LogType.INFO);
        return new ParseResult(node, _index);
    }

    private void CreateComparisonOperation(out INode node, INode LeftNode, out Token OperatorToken)
    {
        OperatorToken = _tokens[_index += 1];
        INode RightNode = new ValueNode(_tokens[_index += 1]);
        node = new ComparisonOperationNode(LeftNode, OperatorToken, RightNode);

        if (_tokens.Count != _index + 1)
        {
            if (_tokens[_index + 1].TokenGroup == TokenGroup.COMPARISONOPERATORS)
            {
                var result = CreateBinaryOperationNode(_index, RightNode);
                RightNode = result.node;
                _index = result.index;

                node = new ComparisonOperationNode(LeftNode, OperatorToken, RightNode);
            }
        }
    }

    /// <summary>
    /// Creates a Binary Operation Node.
    /// When another target is found creates another parent node
    /// </summary>
    private INode ExploreBinaryOperation(ref INode LeftNode)
    {
        INode node;
        if (_tokens.Count == _index + 1) return LeftNode;
        if (_tokens[_index + 1].TokenGroup != TokenGroup.OPERATORS) return LeftNode;
        while (true)
        {
            var result = CreateBinaryOperationNode(_index, LeftNode);
            node = result.node;
            _index = result.index;

            LeftNode = node;
            if (_tokens.Count != _index + 1)
            {
                if (_tokens[_index + 1].TokenGroup == TokenGroup.OPERATORS)
                {
                    continue;
                }
            }
            break;
        }

        return node;
    }

    private ParseResult ReturnValueNode(INode LeftNode)
    {
        Logger.Log("Returning Single ValueNode", LogType.INFO);
        return new ParseResult(LeftNode, _index);
    }

    private (INode node, int index) CreateBinaryOperationNode(int index, INode LeftNode)
    {
        INode node = null;
        var OperatorNode = _tokens[index += 1];
        INode RightNode;
        if(_tokens[_index+1].TokenType.Equals(TokenType.LEFTBRACKET) || !_tokens[_index+1].TokenType.Equals(TokenType.LEFTPARENTHESES))
        {
            var parser = _parserFactory.GetParser(_index, _tokens, Logger);
            var result = parser.CreateNode();
            RightNode = result.Node;
        }
        else
        {
            RightNode = new ValueNode(_tokens[_index]);
        }
        node = new BinaryOperationNode(LeftNode, OperatorNode, RightNode);
        return (node, index);
    }
}
