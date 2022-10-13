using Parser.Node;
using Parser.Parsers;
using Parser.Parsers.Interfaces;
using Lexer.Enums;
using Lexer.Tokens;
using Parser.Node.Interfaces;
using Common;
using Common.Enum;

namespace Parser;
public class Parser
{

    private List<Token> _tokens;
    private IParserFactory parserFactory = new ParserFactory();
    private ITokenParser tokenParser;
    public Logger Logger { get; set; }
    public ObjectSerializer ObjectSerializer { get; set; }
    public string FileName { get; set; }
    public Parser(List<Token> tokens, Logger logger, ObjectSerializer objectSerializer, string fileName)
    {
        _tokens = tokens;
        Logger = logger;
        ObjectSerializer = objectSerializer;
        FileName = fileName;
        logger.Log("Created Parser", this.GetType().Name, LogType.INFO);
    }
    public Scope StartParse()
    {
        var index = 0;
        Scope scope = new Scope(Logger);
        while (index + 1 <= _tokens.Count())
        {
            if(_tokens == null) 
            {
                Logger.Log("No Tokens Found", this.GetType().Name, LogType.ERROR);
                throw new ArgumentNullException(nameof(_tokens));
            }
            var tokenParser = parserFactory.GetParser(_tokens[index], _tokens, Logger);
            var parseResult = tokenParser.CreateNode();

            Logger.Log($"Created {parseResult.node.GetType().Name} | \"{parseResult.node.ToString()}\"", this.GetType().Name, LogType.INFO);

            scope.AddNode(parseResult.node);
            index = parseResult.index;
            index++;
        }

        Logger.Log("Finished Parsing", this.GetType().Name, LogType.INFO);

        ObjectSerializer.SerializeObject(scope, $"{FileName}.pirate");

        return scope;
    }

}