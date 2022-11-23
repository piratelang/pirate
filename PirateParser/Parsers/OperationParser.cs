using PirateParser.Node;
using PirateParser.Parsers.Interfaces;
using PirateParser.Node.Interfaces;

namespace PirateParser.Parsers;

public class OperationParser : BaseParser, ITokenParser
{

    public OperationParser(List<Token> tokens, int index, ILogger logger) : base(tokens, index, logger) {}

    public override (INode node, int index) CreateNode()
    {
        INode node = null;

        INode LeftNode = new ValueNode(_tokens[_index]);

        if (_tokens.Count == _index + 1)
        {
            Logger.Log("Returning Single ValueNode", LogType.INFO);
            return (LeftNode, _index);
        }

        var OperatorNode = _tokens[_index + 1];

        if (OperatorNode.TokenGroup != TokenGroup.OPERATORS && OperatorNode.TokenGroup != TokenGroup.COMPARISONOPERATORS)
        {
            Logger.Log("Returning Single ValueNode", LogType.INFO);
            return (LeftNode, _index);
        }

        if (OperatorNode.TokenGroup == TokenGroup.OPERATORS)
        {
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
        }
        if (_tokens.Count == _index + 1)
        {
            Logger.Log("Returning Binary Operation Node", LogType.INFO);
            if(node == null)
            {
                Logger.Log("Node provided is null", LogType.ERROR);
                throw new NullReferenceException("Node provided is null");
            }
            return (node, _index);
        }

        OperatorNode = _tokens[_index + 1];
        if (OperatorNode.TokenGroup == TokenGroup.COMPARISONOPERATORS)
        {
            OperatorNode = _tokens[_index += 1];
            INode RightNode = new ValueNode(_tokens[_index += 1]);
            node = new ComparisonOperationNode(LeftNode, OperatorNode, RightNode);

            if (_tokens.Count != _index + 1)
            {
                if (_tokens[_index + 1].TokenGroup == TokenGroup.OPERATORS)
                {
                    var result = CreateBinaryOperationNode(_index, RightNode);
                    RightNode = result.node;
                    _index = result.index;

                    node = new ComparisonOperationNode(LeftNode, OperatorNode, RightNode);
                }
            }
        }

        Logger.Log("Returning Comparison or Binary Operation Node", LogType.INFO);
        if(node == null)
        {
            Logger.Log("Node provided is null", LogType.ERROR);
            throw new NullReferenceException("Node provided is null");
        }
        return (node, _index);
    }

    private (INode node, int index) CreateBinaryOperationNode(int index, INode LeftNode)
    {
        INode node = null;
        var OperatorNode = _tokens[index += 1];
        var RightNode = new ValueNode(_tokens[index += 1]);
        node = new BinaryOperationNode(LeftNode, OperatorNode, RightNode);
        return (node, index);
    }
}
