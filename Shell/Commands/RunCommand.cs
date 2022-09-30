
using PirateInterpreter;
using PirateLexer;
using PirateParser;

namespace Shell.Commands
{
    public class RunCommand : ICommand
    {
        public string version { get; set; }
        public RunCommand(string Version)
        {
            version = Version;
        }
        public void Run(string[] arguments)
        {
            var fileArgument = "main";
            if (arguments.Length >= 2) { fileArgument = arguments[1]; }

            var exists = File.Exists($"./{fileArgument}.pirate");

            if (!exists)
            {
                Error($"File \"{fileArgument}\" not provided or does not exist.");
                return;
            }

            var location = $"bin/pirate{version}";

            var fileName = fileArgument.Replace(".pirate", "");
            var text = File.ReadAllText(fileName + ".pirate");

            var lexer = new Lexer("test", text);
            var tokens = lexer.MakeTokens();
            if (tokens.tokens.Count() == 0)
            {
                Error("Error occured while lexing tokens.");
            }

            var parser = new Parser(tokens.tokens);
            var parseResult = parser.Parse(location);
            if (parseResult != true)
            {
                Error("Error occured while parsing tokens.");
            }

            var pythonEngine = new PythonEngine(location);
            var result = pythonEngine.InvokeMain("main");
        }

        public void Help()
        {
            Console.WriteLine(String.Join(
                Environment.NewLine,
                "Description",
                "   pirate run command",
                "\nUsage",
                "   pirate run [filename]",
                "\nOptions",
                "   -h --help   Show command line help."
            ));
        }

        public void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n{message}");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}