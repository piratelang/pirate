using Common;
using PirateLexer;
using PirateParser;

Console.WriteLine("Hello, World!");
var Logger = new Logger(new FileWriteHandler(), "Test");

while (true)
{
    var input = Console.ReadLine();
    var lexer = new Lexer(Logger, new TokenRepository(new KeyWordService()));
    var tokens = lexer.MakeTokens(input, "test");

    ObjectSerializer objectSerializer = new(Logger);

    var parser = new Parser(Logger, objectSerializer);
    var parseResult = parser.StartParse(tokens, "Test");
    if (parseResult == null)
    {
        Console.WriteLine("Why is this null?");
    }
    foreach (var node in parseResult.Nodes)
    {
        Console.WriteLine(node.ToString());
    }


}
