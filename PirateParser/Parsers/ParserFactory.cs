using PirateParser.Parsers.Interfaces;

namespace PirateParser.Parsers;

public class ParserFactory: IParserFactory
{
    public BaseParser GetParser(int index, List<Token> tokens, ILogger logger)
    {
        switch(tokens[index].TokenType)
        {
            case TokenControlKeyword.IF:
                return new IfStatementParser(tokens, index, logger, this);
            case TokenControlKeyword.WHILE:
                return new WhileLoopStatementParser(tokens, index, logger, this);
            case TokenControlKeyword.FOR:
                return new ForLoopStatementParser(tokens, index, logger, this);
            case TokenTypeKeyword:
                return new VariableDeclarationParser(tokens, index, logger, this);

            case TokenSyntax.IDENTIFIER:
                return new VariableAssignmentParser(tokens, index, logger, this);
            case TokenValue:
                return new OperationParser(tokens, index, logger);
            
        }  
        throw new ArgumentNullException("node", $"Factory cannot find parser for {tokens[index].GetType().Name}");
    }
}
