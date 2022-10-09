using NewPirateLexer.Enums;
using NewPirateLexer.Tokens;

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
    public void StartParse()
    {
        if(_tokens == null) throw new ArgumentNullException(nameof(_tokens));
        // start token 1
        var tokenParser = parserFactory.GetParser(_tokens.First(), _tokens);
    }

}