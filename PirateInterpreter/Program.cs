using Common;
using PirateInterpreter;
using PirateLexer;
using PirateParser;

Console.WriteLine("Hello, World!");
var Logger = new Logger("Test");

while (true)
{
    var input = Console.ReadLine();
    var lexer = new Lexer(Logger);
    var result = lexer.MakeTokens(input, "test");

    if (result.tokens == null && result.error != null)
    {
        Console.WriteLine(result.error.ToString());
        return;
    }

    ObjectSerializer objectSerializer = new(Logger);

    var parser = new Parser(Logger, objectSerializer);
    var parseResult = parser.StartParse(result.tokens, "Test");

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
