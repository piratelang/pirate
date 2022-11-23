using PirateParser.Parsers;
using PirateParser.Parsers.Interfaces;

namespace PirateParser;

public class Parser : IParser
{
    private IParserFactory parserFactory = new ParserFactory();
    public ILogger Logger { get; set; }
    public IObjectSerializer ObjectSerializer { get; set; }

    public Parser(ILogger logger, IObjectSerializer objectSerializer)
    {
        Logger = logger;
        ObjectSerializer = objectSerializer;
        logger.Log("Created Parser", LogType.INFO);
    }
    public Scope StartParse(List<Token> tokens, string fileName)
    {
        var index = 0;
        Scope scope = new(Logger);
        while (index + 1 <= tokens.Count())
        {
            if (tokens == null)
            {
                Logger.Log("No Tokens Found", LogType.ERROR);
                throw new ArgumentNullException(nameof(tokens));
            }
            var tokenParser = parserFactory.GetParser(index, tokens, Logger);
            var parseResult = tokenParser.CreateNode();

            Logger.Log($"Created {parseResult.node.GetType().Name} | \"{parseResult.node.ToString()}\"", LogType.INFO);

            scope.AddNode(parseResult.node);
            index = parseResult.index;
            index++;
            if (index + 1 <= tokens.Count())
            {
                if (tokens[index].TokenType.Equals(TokenType.SEMICOLON))
                {
                    index++;
                }
            }
        }

        Logger.Log("Finished Parsing", LogType.INFO);

        ObjectSerializer.SerializeObject(scope, $"{fileName}.pirate");

        return scope;
    }

}