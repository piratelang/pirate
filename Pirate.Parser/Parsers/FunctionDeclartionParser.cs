using Pirate.Parser.Node;
using Pirate.Parser.Node.Interfaces;

namespace Pirate.Parser.Parsers;

/// <summary>
/// A parser looking for a function declaration.
/// Defines a Identifier followed by a list of parameters, a return type and a list of body statements.
/// </summary>
public class FunctionDeclartionParser : BaseParser
{
    public FunctionDeclartionParser(List<Token> tokens, int index, ILogger logger) : base(tokens, index, logger) { }

    public override ParseResult CreateNode()
    {
        INode node;

        var functionToken = _tokens[_index];
        if (!functionToken.Matches(TokenType.FUNC)) throw new ParserException("No Function Declaration was found");

        var identifierNode = new ValueNode(_tokens[_index += 1]);

        if (!_tokens[_index += 1].Matches(TokenType.LEFTPARENTHESES)) throw new ParserException("No Left Parenthesis was found");

        List<IParameterDefinitionNode> parameters = CreateParameterDefinitionNodes();

        if (!_tokens[_index += 1].Matches(TokenType.COLON)) throw new ParserException("No Colon was found");

        var returnTypeToken = _tokens[_index += 1];

        if (!_tokens[_index += 1].Matches(TokenType.LEFTCURLYBRACE)) throw new ParserException("No Left Curly Braces was found");
        List<INode> Nodes = CreateBodyNodes();

        if (_tokens[_index].Matches(TokenType.RETURN))
        {
            return CreateNodeWithReturn(out node, identifierNode, parameters, returnTypeToken, Nodes);
        }

        node = new FunctionDeclarationNode(identifierNode, parameters, returnTypeToken, Nodes);
        return new ParseResult(node, _index);
    }

    private ParseResult CreateNodeWithReturn(out INode node, ValueNode identifierNode, List<IParameterDefinitionNode> parameters, Token returnTypeToken, List<INode> Nodes)
    {
        var valueNode = new ParserFactory().GetParser(_index += 1, _tokens, Logger);
        var result = valueNode.CreateNode();

        _index = result.Index;
        node = new FunctionDeclarationNode(identifierNode, parameters, returnTypeToken, Nodes, result.Node);

        if (!_tokens[_index += 1].Matches(TokenType.SEMICOLON)) throw new ParserException("No Semicolon was found");
        if (!_tokens[_index += 1].Matches(TokenType.RIGHTCURLYBRACE)) throw new ParserException("No Right Curly Braces was found");

        return new ParseResult(node, _index);
    }

    private List<INode> CreateBodyNodes()
    {
        List<INode> Nodes = new List<INode>();
        while (!_tokens[_index += 1].Matches(TokenType.RIGHTCURLYBRACE))
        {
            if (_tokens[_index].Matches(TokenType.RETURN)) break;
            var parser = new ParserFactory().GetParser(_index, _tokens, Logger);
            var result = parser.CreateNode();
            Nodes.Add(result.Node);
            _index = result.Index;
            if (_tokens[_index + 1].TokenType.Equals(TokenType.SEMICOLON))
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
        while (!_tokens[_index += 1].Matches(TokenType.RIGHTPARENTHESES))
        {
            var parameter = new ParameterDefinitionNode(_tokens[_index], new ValueNode(_tokens[_index += 1]));
            parameters.Add(parameter);

            if (_tokens[_index += 1].Matches(TokenType.COMMA)) continue;
            if (_tokens[_index].Matches(TokenType.RIGHTPARENTHESES)) break;
            throw new ParserException("No Right Parenthesis was found");
        }

        return parameters;
    }
}