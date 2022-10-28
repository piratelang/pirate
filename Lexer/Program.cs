using Common;

Console.WriteLine("Hello, World!");

var Logger = new Logger("Test");

var input = Console.ReadLine();
var result = Lexer.Lexer.Instance(Logger).MakeTokens(input, "test");

foreach (var item in result)
    {
        Console.WriteLine(item.ToString());
    }
