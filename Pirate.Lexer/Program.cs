using Pirate.Lexer;
using Pirate.Lexer.Tokens;

Console.WriteLine("Hello, World!");

var logger = new Logger();

var input = Console.ReadLine();
var lexer = new Lexer(logger, new TokenRepository(new KeyWordService()));
var result = lexer.MakeTokens(input, "test");
foreach (var item in result)
    {
        Console.WriteLine(item.ToString());
    }
