using PirateParser.Parsers.Interfaces;

namespace PirateParser.Parsers;

public class ParserFactory: IParserFactory
{
    public BaseParser GetParser(Token token, List<Token> tokens, ILogger logger)
    {
        switch(token.TokenType)
        {
            case TokenControlKeyword.IF:
                return new IfStatementParser(tokens, token, logger, this);
            case TokenControlKeyword.WHILE:
                return new WhileLoopStatementParser(tokens, token, logger, this);
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
