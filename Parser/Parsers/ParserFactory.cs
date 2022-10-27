using Lexer.Tokens;
using Lexer.Enums;
using Parser.Parsers.Interfaces;
using Common.Interfaces;

namespace Parser.Parsers;

public class ParserFactory: IParserFactory
{
    public ITokenParser GetParser(Token token, List<Token> tokens, ILogger logger)
    {
        switch(token.TokenType)
        {
            case TokenTypeKeyword:
                return new VariableAssignParser(tokens, token, logger, this);

            case TokenSyntax.IDENTIFIER:
                return new OperationParser(tokens, token, logger);
            case TokenValue:
                return new OperationParser(tokens, token, logger);
            
        }  
        throw new ArgumentNullException("node", $"Factory cannot find parser for {token.GetType().Name}");
    }
}
