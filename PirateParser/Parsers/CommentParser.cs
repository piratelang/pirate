namespace PirateParser.Parsers;

public class CommentParser : BaseParser
{
    public CommentParser(List<Token> tokens, int index, ILogger logger) : base(tokens, index, logger)
    {
    }

    public override ParseResult CreateNode()
    {
        if (!_tokens[_index].Matches(TokenType.DOUBLEDIVIDE)) throw new ParserException("No Comment was found");
        _index++;

        while(_tokens[_index].TokenType is not TokenType.SEMICOLON)
        {
            // Save comment and return comment node
            //Skip comment in interpreter
        }
        return new ParseResult(null, _index+1);
    }
}