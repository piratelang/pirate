// See https://aka.ms/new-console-template for more information
using Interpreter;
using Parser;
using Lexer;

Console.WriteLine("Hello, World!");
while (true)
{
    var input = Console.ReadLine();
    var lexer = new Lexer.Lexer("test", input);
    var result = lexer.MakeTokens();

    if (result.tokens == null)
    {
        Console.WriteLine(result.error.ToString());
    }

    var parser = new Parser.Parser(result.tokens);
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
