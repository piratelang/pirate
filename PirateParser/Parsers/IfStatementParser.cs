using PirateParser.Node;
using PirateParser.Node.Interfaces;

namespace PirateParser.Parsers
{
    public class IfStatementParser : BaseParser
    {
        private ParserFactory _parserFactory { get; set; }

        public IfStatementParser(List<Token> tokens, Token token, ILogger logger, ParserFactory parserFactory) : base(tokens, token, logger)
        {
            _parserFactory = parserFactory;
        }

        public override (INode node, int index) CreateNode()
        {
            INode node;

            var index = _tokens.IndexOf(_currentToken);

            if (!_currentToken.Matches(TokenControlKeyword.IF))
            {
                Logger.Log("No If Statement was found", this.GetType().Name, LogType.ERROR);
                throw new ParserException("No If Statement was found");
            }

            if (!_tokens[index += 1].Matches(TokenSyntax.LEFTPARENTHESES))
            {
                Logger.Log("No Left Parentheses was found", this.GetType().Name, LogType.ERROR);
                throw new ParserException("No Left Parentheses was found");
            }

            var parser = _parserFactory.GetParser(_tokens[index += 1], _tokens, Logger);
            var result = parser.CreateNode();
            if (result.node is not IOperationNode)
            {
                Logger.Log("If Statement does not contain a valid operation", this.GetType().Name, LogType.ERROR);
                throw new ParserException("If Statement does not contain a valid operation");
            }

            IOperationNode Operation = (IOperationNode)result.node;
            index = result.index;

            if (!_tokens[index += 1].Matches(TokenSyntax.RIGHTPARENTHESES))
            {
                Logger.Log("No Right Parentheses was found", this.GetType().Name, LogType.ERROR);
                throw new ParserException("No Right Parentheses was found");
            }

            if (!_tokens[index += 1].Matches(TokenSyntax.LEFTCURLYBRACE))
            {
                Logger.Log("No Left Curly Brace was found", this.GetType().Name, LogType.ERROR);
                throw new ParserException("No Left Curly Braces was found");
            }

            List<INode> Nodes = new List<INode>();
            while (!_tokens[index += 1].Matches(TokenSyntax.RIGHTCURLYBRACE))
            {
                parser = _parserFactory.GetParser(_tokens[index], _tokens, Logger);
                result = parser.CreateNode();
                Nodes.Add(result.node);
                index = result.index;
            }

            node = new IfStatementNode(Operation, Nodes);
            return (node, index);
        }
    }
}