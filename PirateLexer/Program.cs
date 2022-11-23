using Common;
using PirateLexer;
using PirateLexer.Tokens;

Console.WriteLine("Hello, World!");

var Logger = new Logger(new FileWriteHandler(), new EnvironmentVariables(new FileReadHandler(), new FileWriteHandler()), "Test");

var input = Console.ReadLine();
var lexer = new Lexer(Logger, new TokenRepository(new KeyWordService()));
var result = lexer.MakeTokens(input, "test");
foreach (var item in result)
    {
        Console.WriteLine(item.ToString());
    }
