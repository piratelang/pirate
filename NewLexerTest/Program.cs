using NewPirateLexer;

Console.WriteLine("Hello, World!");

var input = Console.ReadLine();
var lexer = new Lexer("test", input);
var result = lexer.MakeTokens();

if (result.error != null)
{
    Console.WriteLine(result.error.AsString());
}
else
{
    foreach (var item in result.tokens)
    {
        Console.WriteLine(item.Display());
    }
}