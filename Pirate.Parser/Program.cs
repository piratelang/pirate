using Pirate.Lexer;
using Pirate.Parser;

Console.WriteLine("Hello, World!");
var Logger = new Logger();

while (true)
{
    var input = Console.ReadLine();
    var lexer = new Lexer(Logger, new TokenRepository(new KeyWordService()));
    var tokens = lexer.MakeTokens(input, "test").ToList();

    var fileReadHandler = new FileReadHandler();
    var fileWriteHandler = new FileWriteHandler();
    ObjectSerializer objectSerializer = new(Logger, new EnvironmentVariables(fileReadHandler, fileWriteHandler), fileWriteHandler, fileReadHandler);

    var parser = new Parser(Logger);
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
