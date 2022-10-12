using Parser.Node;
using Lexer.Tokens;
using Parser.Parsers.Interfaces;
using Parser.Node.Interfaces;
using Lexer.Enums;

namespace Parser.Parsers;

public class OperationParser : ITokenParser
{
    private List<Token> _tokens;
    private Token _currentToken;

    public OperationParser(List<Token> tokens, Token currentToken)
    {
        _tokens = tokens;
        _currentToken = currentToken;
    }

    public (INode node, int index) CreateNode()
    {
        INode node = null;

        var index = _tokens.IndexOf(_currentToken);
        INode LeftNode = new ValueNode(_tokens[index]);

        if (_tokens.Count == index + 1)
        {
            return (LeftNode, index);
        }

        var OperatorNode = _tokens[index + 1];

        if (OperatorNode.TokenGroup != TokenGroup.OPERATORS && OperatorNode.TokenGroup != TokenGroup.COMPARISONOPERATORS)
        {
            return (LeftNode, index);
        }

        if (OperatorNode.TokenGroup == TokenGroup.OPERATORS)
        {
            while (true)
            {
                var result = CreateBinaryOperationNode(index, LeftNode);
                node = result.node;
                index = result.index;

                LeftNode = node;
                if (_tokens.Count != index + 1)
                {
                    if (_tokens[index + 1].TokenGroup == TokenGroup.OPERATORS)
                    {
                        continue;
                    }
                }
                break;
            }
        }
        if (_tokens.Count == index + 1)
        {
            return (node, index);
        }

        OperatorNode = _tokens[index + 1];
        if (OperatorNode.TokenGroup == TokenGroup.COMPARISONOPERATORS)
        {
            OperatorNode = _tokens[index += 1];
            INode RightNode = new ValueNode(_tokens[index += 1]);
            node = new ComparisonOperationNode(LeftNode, OperatorNode, RightNode);

            if (_tokens.Count != index + 1)
            {
                if (_tokens[index + 1].TokenGroup == TokenGroup.OPERATORS)
                {
                    var result = CreateBinaryOperationNode(index, RightNode);
                    RightNode = result.node;
                    index = result.index;

                    node = new ComparisonOperationNode(LeftNode, OperatorNode, RightNode);
                }
            }
        }
        return (node, index);
    }

    public (INode node, int index) CreateBinaryOperationNode(int index, INode LeftNode)
    {
        INode node = null;
        var OperatorNode = _tokens[index += 1];
        var RightNode = new ValueNode(_tokens[index += 1]);
        node = new BinaryOperationNode(LeftNode, OperatorNode, RightNode);
        return (node, index);
    }
}
