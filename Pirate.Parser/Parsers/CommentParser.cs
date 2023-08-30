using Pirate.Parser.Node;

namespace Pirate.Parser.Parsers;

/// <summary>
/// A parser for comments.
/// </summary>
public class CommentParser : BaseParser
{
    public CommentParser(List<Token> tokens, int index, ILogger logger) : base(tokens, index, logger)
    {
    }

    public override ParseResult CreateNode()
    { 
        if (!_tokens[_index].Matches(TokenType.DOUBLEDIVIDE)) throw new ParserException("No Comment was found");
        _index++;

        List<Token> comment = new();
        while (_tokens[_index].TokenType is not TokenType.SEMICOLON)
        {
            comment.Add(_tokens[_index]);
            _index++;
        }
        return new ParseResult(new CommentNode(comment), _index + 1);
    }
}