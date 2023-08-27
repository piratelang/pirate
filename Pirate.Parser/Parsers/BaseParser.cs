using Pirate.Common.Interfaces;
using Pirate.Lexer.Tokens;

namespace Pirate.Parser.Parsers;

/// <summary>
/// A base class for all parsers.
/// </summary>
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

    public abstract ParseResult CreateNode();
}