using System.Threading;
using Lexer;
using Common;

Console.WriteLine("Hello, World!");

var Logger = new Logger("Test");

var input = Console.ReadLine();
var lexer = new Lexer.Lexer("test", input, Logger);
var result = lexer.MakeTokens();

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