// See https://aka.ms/new-console-template for more information
using Parser;
using Lexer;
using Common;

Console.WriteLine("Hello, World!");
var Logger = new Logger("Test");

while (true)
{
    var input = Console.ReadLine();
    var lexer = new Lexer.Lexer("test", input, Logger);
    var result = lexer.MakeTokens();

    if(result.tokens == null)
    {
        Console.WriteLine(result.error.ToString());
    }

    var parser = new Parser.Parser(result.tokens, Logger);
    var parseResult = parser.StartParse();
    if(parseResult == null)
    {
        Console.WriteLine("Why is this null?");
    }
    foreach (var node in parseResult.Nodes)
    {
        Console.WriteLine(node.Display());
    }
    

}
