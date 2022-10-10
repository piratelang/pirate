using NewPirateLexer.Tokens;
using NewPirateLexer.Enums;
using NewLexerTest.Enums;
using NewParserTest.Parsers.Interfaces;
using NewParserTest.Node.Interfaces;

namespace NewParserTest.Parsers;

public class ParserFactory: IParserFactory
{
    public ITokenParser GetParser(Token token, List<Token> tokens)
    {
        switch(token.TokenType)
        {
            case TokenTypeKeyword.VAR:
                return new VariableAssignParser(tokens, token);

            case TokenSyntax.IDENTIFIER:
                return new OperationParser(tokens, token);
            case TokenValue:
                return new OperationParser(tokens, token);
            
        }  
        return null;   
    }
}
