// See https://aka.ms/new-console-template for more information
using PirateLexer;

Console.WriteLine("Hello, World!");

var text = File.ReadAllText("./Pirate/test.pirate");
var newText = text.Replace("\r", "");
var lexer = new Lexer("test", newText);
var tokens = lexer.MakeTokens();

foreach (var item in tokens.tokens)
{
    Console.WriteLine(item.Display());
}
Console.ReadLine();