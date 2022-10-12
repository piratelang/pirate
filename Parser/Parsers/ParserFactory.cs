using Lexer.Tokens;
using Lexer.Enums;
using Lexer.Enums;
using Parser.Parsers.Interfaces;
using Parser.Node.Interfaces;

namespace Parser.Parsers;

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
