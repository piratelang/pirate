using PirateParser.Node.Interfaces;

namespace PirateParser.Parsers;

public abstract class BaseParser
{
    protected List<Token> _tokens;
    protected Token _currentToken;
    protected ILogger Logger;

    public BaseParser(List<Token> tokens, Token currentToken, ILogger logger)
    {
        _tokens = tokens;
        _currentToken = currentToken;
        Logger = logger;
    }

    public abstract (INode node, int index) CreateNode();
}