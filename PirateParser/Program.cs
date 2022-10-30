using Common;
using PirateLexer;
using PirateParser;

Console.WriteLine("Hello, World!");
var Logger = new Logger("Test");

while (true)
{
    var input = Console.ReadLine();
    var lexer = new Lexer(Logger);
    var result = lexer.MakeTokens(input, "test");

    if (result.tokens == null)
    {
        Console.WriteLine(result.error.ToString());
    }

    ObjectSerializer objectSerializer = new(Logger);

    var parser = new Parser(Logger, objectSerializer);
    var parseResult = parser.StartParse(result.tokens, "Test");
    if (parseResult == null)
    {
        Console.WriteLine("Why is this null?");
    }
    foreach (var node in parseResult.Nodes)
    {
        Console.WriteLine(node.ToString());
    }


}
