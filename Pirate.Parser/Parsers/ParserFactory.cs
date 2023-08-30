using Pirate.Parser.Parsers.Interfaces;

namespace Pirate.Parser.Parsers;

public class ParserFactory : IParserFactory
{
    public BaseParser GetParser(int index, List<Token> tokens, ILogger logger)
    {
        switch (tokens[index].TokenType)
        {
            case TokenType.FUNC:
                return new FunctionDeclartionParser(tokens, index, logger);
            case TokenType.IF:
                return new IfStatementParser(tokens, index, logger, this);
            case TokenType.WHILE:
                return new WhileLoopStatementParser(tokens, index, logger, this);
            case TokenType.FOR:
                return new ForLoopStatementParser(tokens, index, logger, this);
            case TokenType.VAR when tokens[index].TokenGroup == TokenGroup.TYPEKEYWORD:
            case TokenType.STRING when tokens[index].TokenGroup == TokenGroup.TYPEKEYWORD:
            case TokenType.INT when tokens[index].TokenGroup == TokenGroup.TYPEKEYWORD:
            case TokenType.FLOAT when tokens[index].TokenGroup == TokenGroup.TYPEKEYWORD:
            case TokenType.CHAR when tokens[index].TokenGroup == TokenGroup.TYPEKEYWORD:
                return new VariableDeclarationParser(tokens, index, logger, this);
            case TokenType.IDENTIFIER:
                return new IdentifierParser(tokens, index, logger, this);
            case TokenType.STRING when tokens[index].TokenGroup == TokenGroup.VALUE:
            case TokenType.INT when tokens[index].TokenGroup == TokenGroup.VALUE:
            case TokenType.FLOAT when tokens[index].TokenGroup == TokenGroup.VALUE:
            case TokenType.CHAR when tokens[index].TokenGroup == TokenGroup.VALUE:
                return new OperationParser(tokens, index, logger);
            case TokenType.DOUBLEDIVIDE:
                return new CommentParser(tokens, index, logger);
        }
        throw new ArgumentNullException("node", $"Factory cannot find parser for {tokens[index].GetType().Name}");
    }
}
