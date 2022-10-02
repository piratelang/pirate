
using PirateInterpreter;
using PirateLexer;
using PirateParser;

namespace Shell.Commands
{
    public class InitCommand : ICommand
    {
        public string version { get; set; }
        public InitCommand(string Version)
        {
            version = Version;
        }
        public void Run(string[] arguments)
        {
            var nameArgument = "main";
            if (arguments.Length == 2) { nameArgument = arguments[1];}

            var fileName = nameArgument.Replace(".pirate", "");
            var file = File.CreateText($"./{fileName}.pirate");
            file.Write(String.Join(
                Environment.NewLine,
                "func main()",
                "{",
                "    print(\"Hello World\");",
                "}"
            ));
            file.Close();
            Console.WriteLine($"\nCreated {fileName}.pirate");
        }

        public void Help()
        {
            Console.WriteLine(String.Join(
                Environment.NewLine,
                "Description",
                "   pirate initalize command",
                "\nUsage",
                "   pirate init [filename]",
                "\nOptions",
                "   -h --help       Show command line help."
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