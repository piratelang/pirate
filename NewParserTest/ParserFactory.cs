using NewPirateLexer.Tokens;
using NewPirateLexer.Enums;
using NewLexerTest.Enums;

namespace NewParserTest;

public class ParserFactory: IParserFactory
{
    public ITokenParser GetParser(Token token, List<Token> tokens)
    {
        switch(token.TokenType)
        {
            case TokenValue:
                return new TokenValueParser(tokens);
            
        }  
          return null;   
    }
}
