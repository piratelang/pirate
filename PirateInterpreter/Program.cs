using Common;
using PirateInterpreter;
using PirateInterpreter.Interpreters;
using PirateInterpreter.StandardLibrary;
using PirateLexer;
using PirateParser;

Console.WriteLine("Hello, World!");
var Logger = new Logger(new FileWriteHandler(), new EnvironmentVariables(new FileReadHandler(), new FileWriteHandler()), "test");

while (true)
{
    var input = Console.ReadLine();
    var lexer = new Lexer(Logger, new TokenRepository(new KeyWordService()));
    var tokens = lexer.MakeTokens(input, "test");

    ObjectSerializer objectSerializer = new(Logger, new EnvironmentVariables(new FileReadHandler(), new FileWriteHandler()));

    var parser = new Parser(Logger, objectSerializer);
    var parseResult = parser.StartParse(tokens, "Test");

    if (parseResult.Nodes == null)
    {
        Console.WriteLine("stuk");
        return;
    }
    var interpreterFactory = new InterpreterFactory(new StandardLibraryFactory(), Logger);
    var interpreter = new Interpreter(objectSerializer, Logger, interpreterFactory);
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
