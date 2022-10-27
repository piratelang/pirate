using Common;

Console.WriteLine("Hello, World!");

var Logger = new Logger("Test");

var input = Console.ReadLine();
var lexer = new Lexer.Lexer(Logger);
var result = lexer.MakeTokens(input, "test");

if (result.error != null)
{
    Console.WriteLine(result.error.AsString());
}
else
{
    foreach (var item in result.tokens)
    {
        Console.WriteLine(item.ToString());
    }
}