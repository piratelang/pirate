
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
            var fileName = fileArgument.Replace(".pirate", "");
            var exists = File.Exists($"./{fileName}.pirate");

            if (!exists)
            {
                Error($"File \"{fileArgument}\" not provided or does not exist.");
                return;
            }

            var buildCommand = new BuildCommand(version);
            buildCommand.Run(arguments);

            var location = $"bin/pirate{version}";
            var pythonEngine = new PythonEngine($"{location}/{fileName}.py");
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