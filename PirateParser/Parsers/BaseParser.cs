using PirateParser.Node.Interfaces;

namespace PirateParser.Parsers;

public abstract class BaseParser
{
    protected List<Token> _tokens;
    protected int _index;
    protected ILogger Logger;

    public BaseParser(List<Token> tokens, int index, ILogger logger)
    {
        _tokens = tokens;
        _index = index;
        Logger = logger;
    }

    public abstract (INode node, int index) CreateNode();
}