using Pirate.Parser.Parsers.Interfaces;
using Pirate.Parser.Parsers;
using Pirate.Lexer.TokenType.Enums;
using Pirate.Parser.Interfaces;

namespace Pirate.Parser;

/// <summary>
/// A starting point for parsing.
/// </summary>
public class Parser : IParser
{
    private IParserFactory parserFactory = new ParserFactory();
    public ILogger Logger { get; set; }
    public IObjectSerializer ObjectSerializer { get; set; }

    public Parser(ILogger logger, IObjectSerializer objectSerializer)
    {
        Logger = logger;
        ObjectSerializer = objectSerializer;
        logger.Info("Created Parser");
    }
    public Scope StartParse(List<Token> tokens, string fileName)
    {
        var index = 0;
        Scope scope = new(Logger);
        while (index + 1 <= tokens.Count())
        {
            if (tokens == null)
            {
                Logger.Info("No Tokens Found");
                throw new ArgumentNullException(nameof(tokens));
            }
            var tokenParser = parserFactory.GetParser(index, tokens, Logger);
            var parseResult = tokenParser.CreateNode();

            if (parseResult != null)
            {
                Logger.Info($"Created {parseResult.Node.GetType().Name} | \"{parseResult.Node.ToString()}\"");
                scope.AddNode(parseResult.Node);
                index = parseResult.Index;
            }
            index++;
            if (index + 1 <= tokens.Count())
            {
                if (tokens[index].TokenType.Equals(TokenType.SEMICOLON))
                {
                    index++;
                }
            }
        }

        Logger.Info("Finished Parsing");

        ObjectSerializer.SerializeObject(scope, $"{fileName}.pirate");

        return scope;
    }

}