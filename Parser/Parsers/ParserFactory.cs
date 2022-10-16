using Lexer.Tokens;
using Lexer.Enums;
using Lexer.Enums;
using Parser.Parsers.Interfaces;
using Parser.Node.Interfaces;
using Common;

namespace Parser.Parsers;

public class ParserFactory: IParserFactory
{
    public ITokenParser GetParser(Token token, List<Token> tokens, ILogger logger)
    {
        switch(token.TokenType)
        {
            case TokenTypeKeyword.VAR:
                return new VariableAssignParser(tokens, token, logger, this);

            case TokenSyntax.IDENTIFIER:
                return new OperationParser(tokens, token, logger);
            case TokenValue:
                return new OperationParser(tokens, token, logger);
            
        }  
        return null;   
    }
}
