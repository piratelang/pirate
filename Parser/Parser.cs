using Parser.Parsers;
using Parser.Parsers.Interfaces;
using Lexer.Enums;
using Lexer.Tokens;
using Common.Enum;
using Common.Interfaces;

namespace Parser;

public class Parser : IParser
{
    private IParserFactory parserFactory = new ParserFactory();
    public ILogger Logger { get; set; }
    public IObjectSerializer ObjectSerializer { get; set; }

    public Parser(ILogger logger, IObjectSerializer objectSerializer)
    {
        Logger = logger;
        ObjectSerializer = objectSerializer;
        logger.Log("Created Parser", this.GetType().Name, LogType.INFO);
    }
    public Scope StartParse(List<Token> tokens, string fileName)
    {
        var index = 0;
        Scope scope = new(Logger);
        while (index + 1 <= tokens.Count())
        {
            if (tokens == null)
            {
                Logger.Log("No Tokens Found", this.GetType().Name, LogType.ERROR);
                throw new ArgumentNullException(nameof(tokens));
            }
            var tokenParser = parserFactory.GetParser(tokens[index], tokens, Logger);
            var parseResult = tokenParser.CreateNode();

            Logger.Log($"Created {parseResult.node.GetType().Name} | \"{parseResult.node.ToString()}\"", this.GetType().Name, LogType.INFO);

            scope.AddNode(parseResult.node);
            index = parseResult.index;
            index++;
            if (index + 1 <= tokens.Count())
            {
                if (tokens[index].TokenType.Equals(TokenSyntax.SEMICOLON))
                {
                    index++;
                }
            }
        }

        Logger.Log("Finished Parsing", this.GetType().Name, LogType.INFO);

        ObjectSerializer.SerializeObject(scope, $"{fileName}.pirate");

        return scope;
    }

}