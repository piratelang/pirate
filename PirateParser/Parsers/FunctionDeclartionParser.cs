using PirateParser.Node;
using PirateParser.Node.Interfaces;

namespace PirateParser.Parsers;

public class FunctionDeclartionParser : BaseParser
{
    public FunctionDeclartionParser(List<Token> tokens, int index, ILogger logger) : base(tokens, index, logger) { }
    
    public override (INode node, int index) CreateNode()
    {
        INode node;

        var functionToken = _tokens[_index];
        if (!functionToken.Matches(TokenControlKeyword.FUNC)) throw new ParserException("No Function Declaration was found");

        var identifierNode = new ValueNode(_tokens[_index += 1]);

        if (!_tokens[_index += 1].Matches(TokenSyntax.LEFTPARENTHESES)) throw new ParserException("No Left Parenthesis was found");

        List<IParameterDefinitionNode> parameters = CreateParameterDefinitionNodes();

        if (!_tokens[_index += 1].Matches(TokenSyntax.COLON)) throw new ParserException("No Colon was found");

        var returnTypeToken = _tokens[_index += 1];

        if (!_tokens[_index += 1].Matches(TokenSyntax.LEFTCURLYBRACE)) throw new ParserException("No Left Curly Braces was found");
        List<INode> Nodes = CreateBodyNodes();

        if (_tokens[_index].Matches(TokenControlKeyword.RETURN))
        {
            return CreateNodeWithReturn(out node, identifierNode, parameters, returnTypeToken, Nodes);
        }

        node = new FunctionDeclarationNode(identifierNode, parameters, returnTypeToken, Nodes);
        return (node, _index);
    }

    private (INode node, int index) CreateNodeWithReturn(out INode node, ValueNode identifierNode, List<IParameterDefinitionNode> parameters, Token returnTypeToken, List<INode> Nodes)
    {
        var valueNode = new ParserFactory().GetParser(_index += 1, _tokens, Logger);
        var result = valueNode.CreateNode();

        _index = result.index;
        node = new FunctionDeclarationNode(identifierNode, parameters, returnTypeToken, Nodes, result.node);

        if (!_tokens[_index += 1].Matches(TokenSyntax.SEMICOLON)) throw new ParserException("No Semicolon was found");
        if (!_tokens[_index += 1].Matches(TokenSyntax.RIGHTCURLYBRACE)) throw new ParserException("No Right Curly Braces was found");

        return (node, _index);
    }

    private List<INode> CreateBodyNodes()
    {
        List<INode> Nodes = new List<INode>();
        while (!_tokens[_index += 1].Matches(TokenSyntax.RIGHTCURLYBRACE))
        {
            if (_tokens[_index].Matches(TokenControlKeyword.RETURN)) break;
            var parser = new ParserFactory().GetParser(_index, _tokens, Logger);
            var result = parser.CreateNode();
            Nodes.Add(result.node);
            _index = result.index;
            if (_tokens[_index + 1].TokenType.Equals(TokenSyntax.SEMICOLON))
            {
                _index++;
            }
            if (_index >= _tokens.Count) break;
        }

        return Nodes;
    }

    private List<IParameterDefinitionNode> CreateParameterDefinitionNodes()
    {
        List<IParameterDefinitionNode> parameters = new();
        while (!_tokens[_index += 1].Matches(TokenSyntax.RIGHTPARENTHESES))
        {
            var parameter = new ParameterDefinitionNode(_tokens[_index], new ValueNode(_tokens[_index += 1]));
            parameters.Add(parameter);

            if (_tokens[_index += 1].Matches(TokenSyntax.COMMA)) continue;
            if (_tokens[_index].Matches(TokenSyntax.RIGHTPARENTHESES)) break;
            throw new ParserException("No Right Parenthesis was found");
        }

        return parameters;
    }
}