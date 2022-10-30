﻿using Common;
using PirateLexer;
using PirateParser;

Console.WriteLine("Hello, World!");
var Logger = new Logger("Test");

while (true)
{
    var input = Console.ReadLine();
    var tokens = Lexer.Instance(Logger).MakeTokens(input, "test");

    ObjectSerializer objectSerializer = new(Logger);

    var parser = new Parser(Logger, objectSerializer);
    var parseResult = parser.StartParse(tokens, "Test");
    if (parseResult == null)
    {
        Console.WriteLine("Why is this null?");
    }
    foreach (var node in parseResult.Nodes)
    {
        Console.WriteLine(node.ToString());
    }


}
