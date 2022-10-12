using Parser.Node;
using Parser.Parsers;
using Parser.Parsers.Interfaces;
using Lexer.Enums;
using Lexer.Tokens;
using Parser.Node.Interfaces;

namespace Parser;
public class Parser
{

    private List<Token> _tokens;
    private IParserFactory parserFactory = new ParserFactory();

    private ITokenParser tokenParser;
    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
    }
    public Scope StartParse()
    {
        var index = 0;
        Scope scope = new Scope();
        while (index + 1 <= _tokens.Count())
        {
            if(_tokens == null) throw new ArgumentNullException(nameof(_tokens));
            var tokenParser = parserFactory.GetParser(_tokens[index], _tokens);
            var parseResult = tokenParser.CreateNode();

            scope.AddNode(parseResult.node);
            index = parseResult.index;
            index++;
        }

        return scope;
    }

}