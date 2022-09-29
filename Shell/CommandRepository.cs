using PirateInterpreter;
using PirateLexer;
using PirateParser;

namespace Shell
{
    public class CommandRepository
    {
        public string version { get; set; }
        public CommandRepository(string Version)
        {
            version = Version;
        }

        public void Run(string fileArgument)
        {
            if (fileArgument == string.Empty)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nFile not provided or does not exist.");
                Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine("\nThe \"pirate run [filename]\" command runs the specified module");
                return;
            }

            var location = $"bin/pirate{version}";

            var fileName = fileArgument.Replace(".pirate", "");
            var text = File.ReadAllText(fileName);

            var lexer = new Lexer("test", text);
            var tokens = lexer.MakeTokens();
            if (tokens.tokens.Count() == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n Error occured while lexing tokens.");
                Console.ForegroundColor = ConsoleColor.White;
            }

            var parser = new Parser(tokens.tokens);
            var parseResult = parser.Parse(location);
            if (parseResult != true)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n Error occured while parsing tokens.");
                Console.ForegroundColor = ConsoleColor.White;
            }

            var pythonEngine = new PythonEngine(location);
            var result = pythonEngine.InvokeMain("main");
        }
        public void New(string type)
        {
            if(type == string.Empty)
            {
                Console.WriteLine("\nThe \"pirate new [type]\" command creates a new file from a template");
                Console.WriteLine("\nOptions");
                Console.WriteLine(" - gitignore");
                Console.WriteLine(" - gitattributes");
                Console.WriteLine("\nFor a new pirate file see \"pirate init\" ");
                return;
            }
            
            var typeOptions = new string[]{
                "gitignore",
                "gitattributes"
            };
            if (!typeOptions.Contains(type))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nSpecified file not able to be created");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\nFor a new pirate file see \"pirate init\" ");
                return;
            }
            if (type == "gitignore")
            {
                var file = File.CreateText($"./.gitignore");
                file.Write("[Bb]in/");
                file.Close();
            }
            if (type == "gitattributes")
            {
                var location = "./.gitattributes";
                var file = File.CreateText(location);
                file.Write("*.pirate linguist-language=Squirrel");
                file.Close();
            }
            Console.WriteLine($"\nCreated new .{type}");
        }
    }
}