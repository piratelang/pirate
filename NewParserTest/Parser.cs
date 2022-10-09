using NewParserTest.Node;
using NewParserTest.Parsers;
using NewParserTest.Parsers.Interfaces;
using NewPirateLexer.Enums;
using NewPirateLexer.Tokens;
using NewParserTest.Node.Interfaces;

namespace NewParserTest;
public class Parser
{

    private List<Token> _tokens;
    private IParserFactory parserFactory = new ParserFactory();

    private ITokenParser tokenParser;
    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
    }
    public INode StartParse()
    {
        if(_tokens == null) throw new ArgumentNullException(nameof(_tokens));
        // start token 1
        var tokenParser = parserFactory.GetParser(_tokens.First(), _tokens);
        var parseResult = tokenParser.CreateNode();

        Console.WriteLine(parseResult.index + 1);
        return parseResult.node;
    }

}