// See https://aka.ms/new-console-template for more information
using Interpreter;
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

    if (result.tokens == null)
    {
        Console.WriteLine(result.error.ToString());
    }

    var parser = new Parser.Parser(result.tokens, Logger);
    var parseResult = parser.StartParse();

    if (parseResult.Nodes == null)
    {
        Console.WriteLine("stuk");
    }

    var interpreter = new Interpreter.Interpreter(parseResult);
    var intResult = interpreter.StartInterpreter();

    if (intResult == null)
    {
        Console.WriteLine("Why is this null?");
    }
    Console.WriteLine(intResult.Value);


}
