using Common;
using PirateInterpreter;
using PirateLexer;
using PirateParser;

Console.WriteLine("Hello, World!");
var Logger = new Logger(new FileWriteHandler(), new EnvironmentVariables(), "test");

while (true)
{
    var input = Console.ReadLine();
    var lexer = new Lexer(Logger, new TokenRepository(new KeyWordService()));
    var tokens = lexer.MakeTokens(input, "test");

    ObjectSerializer objectSerializer = new(Logger, new EnvironmentVariables());

    var parser = new Parser(Logger, objectSerializer);
    var parseResult = parser.StartParse(tokens, "Test");

    if (parseResult.Nodes == null)
    {
        Console.WriteLine("stuk");
        return;
    }

    var interpreter = new Interpreter(objectSerializer, Logger);
    var Result = interpreter.StartInterpreter("Test");

    if (Result == null)
    {
        Console.WriteLine("Why is this null?");
        return;
    }
    foreach (var item in Result)
    {
        Console.WriteLine(item.Value);
    }
}
