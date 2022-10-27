using Parser.Node;
using Parser.Parsers.Interfaces;
using Parser.Node.Interfaces;

namespace Parser.Parsers;

public class OperationParser : ITokenParser
{
    private List<Token> _tokens;
    private Token _currentToken;
    public ILogger Logger { get; set; }

    public OperationParser(List<Token> tokens, Token currentToken, ILogger logger)
    {
        _tokens = tokens;
        _currentToken = currentToken;
        Logger = logger;
        logger.Log("Creating Operation Parser", this.GetType().Name, LogType.INFO);
    }

    public (INode node, int index) CreateNode()
    {
        INode node = null;

        var index = _tokens.IndexOf(_currentToken);
        INode LeftNode = new ValueNode(_tokens[index]);

        if (_tokens.Count == index + 1)
        {
            Logger.Log("Returning Single ValueNode", this.GetType().Name, LogType.INFO);
            return (LeftNode, index);
        }

        var OperatorNode = _tokens[index + 1];

        if (OperatorNode.TokenGroup != TokenGroup.OPERATORS && OperatorNode.TokenGroup != TokenGroup.COMPARISONOPERATORS)
        {
            Logger.Log("Returning Single ValueNode", this.GetType().Name, LogType.INFO);
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
            Logger.Log("Returning Binary Operation Node", this.GetType().Name, LogType.INFO);
            if(node == null)
            {
                Logger.Log("Node provided is null", this.GetType().Name, LogType.ERROR);
                throw new NullReferenceException("Node provided is null");
            }
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

        Logger.Log("Returning Comparison or Binary Operation Node", this.GetType().Name, LogType.INFO);
        if(node == null)
        {
            Logger.Log("Node provided is null", this.GetType().Name, LogType.ERROR);
            throw new NullReferenceException("Node provided is null");
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
