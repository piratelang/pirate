
using Pirate.Parser.Node;
using Pirate.Parser.Node.Interfaces;

namespace Pirate.Parser.Parsers;

public class ExternParser : BaseParser
{
    private ParserFactory _parserFactory;

    public ExternParser(List<Token> tokens, int index, ILogger logger, ParserFactory parserFactory) : base(tokens, index, logger)
    {
        _parserFactory = parserFactory;
    }

    public override ParseResult CreateNode()
    {
        if (_tokens[_index].TokenType != TokenType.EXTERN) throw new ParserException(new ExceptionCode(ExceptionPrefix.PARSER, "005"), new List<string> { _tokens[_index].TokenType.ToString() });

        var externToken = _tokens[_index];
        
        var parser = _parserFactory.GetParser(_index + 1, _tokens, Logger);
        var parseResult = parser.CreateNode();

        if (parseResult.Node is not IValueNode) throw new ParserException(new ExceptionCode(ExceptionPrefix.PARSER, "006"), new List<string> { parseResult.Node.GetType().Name });
        return new ParseResult(
            new ExternNode((IValueNode)parseResult.Node), parseResult.Index
        );
    }
}
