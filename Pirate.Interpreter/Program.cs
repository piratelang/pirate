using Pirate.Interpreter;
using Pirate.Interpreter.Interpreters;
using Pirate.Interpreter.StandardLibrary;
using Pirate.Lexer;
using Pirate.Parser;

Console.WriteLine("Hello, World!");
var logger = new Logger();

while (true)
{
    var input = Console.ReadLine();
    var lexer = new Lexer(logger, new TokenRepository(new KeyWordService()));
    var tokens = lexer.MakeTokens(input, "test");

    var fileReadHandler = new FileReadHandler();
    var fileWriteHandler = new FileWriteHandler();
    ObjectSerializer objectSerializer = new(logger, new EnvironmentVariables(fileReadHandler, fileWriteHandler), fileWriteHandler, fileReadHandler);

    var parser = new Parser(logger, objectSerializer);
    var parseResult = parser.StartParse(tokens, "Test");

    if (parseResult.Nodes == null)
    {
        Console.WriteLine("stuk");
        return;
    }
    var interpreterFactory = new InterpreterFactory(new StandardLibraryCallManager(logger), logger);
    var interpreter = new Interpreter(objectSerializer, logger, interpreterFactory);
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
