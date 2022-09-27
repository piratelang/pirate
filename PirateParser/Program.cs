using PirateLexer;

Console.WriteLine("Hello, World!");

var text = File.ReadAllText("./Pirate/test.pirate");
var newText = text.Replace("\r", "");
var lexer = new Lexer(newText);
var tokens = lexer.MakeTokens();