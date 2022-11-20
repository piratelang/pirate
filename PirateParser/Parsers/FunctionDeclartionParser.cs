using PirateParser.Node;
using PirateParser.Node.Interfaces;

namespace PirateParser.Parsers;

public class FunctionDeclartionParser : BaseParser
{
    public FunctionDeclartionParser(List<Token> tokens, int index, ILogger logger) : base(tokens, index, logger)
    {
    }
    // func name(parameters) : returntype { body }
    public override (INode node, int index) CreateNode()
    {
        INode node;

        var _currentToken = _tokens[_index];

        if (!_currentToken.Matches(TokenControlKeyword.FUNC)) throw new ParserException("No Function Declaration was found");
        
        var identifierNode = new ValueNode(_tokens[_index += 1]);

        if (!_tokens[_index += 1].Matches(TokenSyntax.LEFTPARENTHESES)) throw new ParserException("No Left Parenthesis was found");
        
        List<INode> parameters = new List<INode>();
        while (!_tokens[_index += 1].Matches(TokenSyntax.RIGHTPARENTHESES))
        {
            var parser = new ParserFactory().GetParser(_index, _tokens, Logger);
            var result = parser.CreateNode();
            if (result.node is not ParameterDefinitionNode) throw new ParserException("Function Declaration does not contain a valid parameter");
            
            _index = result.index;
            parameters.Add(result.node);
        }

        if (!_tokens[_index += 1].Matches(TokenSyntax.COLON)) throw new ParserException("No Colon was found");

        var typeToken = _tokens[_index += 1];

        if (!_tokens[_index += 1].Matches(TokenSyntax.LEFTCURLYBRACE)) throw new ParserException("No Left Curly Braces was found");

        List<INode> Nodes = new List<INode>();
        while (!_tokens[_index += 1].Matches(TokenSyntax.RIGHTCURLYBRACE))
        {
            var parser = new ParserFactory().GetParser(_index, _tokens, Logger);
            var result = parser.CreateNode();
            Nodes.Add(result.node);
            _index = result.index;
            if (_tokens[_index+1].TokenType.Equals(TokenSyntax.SEMICOLON))
            {
                _index++;
            }
            if (_index >= _tokens.Count) break;
        }

        node = new FunctionDeclarationNode(identifierNode, parameters, typeToken, Nodes);
        return (node, _index);
    }
}