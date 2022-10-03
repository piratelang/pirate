
using Common;
using Common.Enum;
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
            Logger.Log("Starting Run Command", this.GetType().Name, LogType.INFO);
            var fileArgument = "main";
            if (arguments.Length >= 2) { fileArgument = arguments[1]; }
            var fileName = fileArgument.Replace(".pirate", "");
            var exists = File.Exists($"./{fileName}.pirate");

            if (!exists)
            {
                Logger.Log($"File \"{fileArgument}\" not provided or does not exist.", this.GetType().Name, LogType.ERROR);
                Error($"File \"{fileArgument}\" not provided or does not exist.");
                return;
            }

            Logger.Log("Starting build", this.GetType().Name, LogType.INFO);
            var buildCommand = new BuildCommand(version);
            buildCommand.Run(arguments);
            Logger.Log("Completed Build", this.GetType().Name, LogType.INFO);

            var location = $"bin/pirate{version}";
            Logger.Log($"Executing {fileName}.py", this.GetType().Name, LogType.INFO);
            var pythonEngine = new PythonEngine($"{location}/{fileName}.py");
            var result = pythonEngine.InvokeMain("main"); //Possibly null reference;
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