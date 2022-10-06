using Common;
using Common.Enum;
using PirateInterpreter;

namespace Shell.Commands
{
    public class RunCommand : ICommand
    {
        public string version { get; set; }
        public Logger logger { get; set; }
        public RunCommand(string Version, Logger Logger)
        {
            version = Version;
            logger = Logger;
        }
        public void Run(string[] arguments)
        {
            logger.Log("Starting Run Command", this.GetType().Name, LogType.INFO);
            var fileArgument = "main";
            if (arguments.Length >= 2) { fileArgument = arguments[1]; }
            var fileName = fileArgument.Replace(".pirate", "");
            var exists = File.Exists($"./{fileName}.pirate");

            if (!exists)
            {
                logger.Log($"File \"{fileArgument}\" not provided or does not exist.", this.GetType().Name, LogType.ERROR);
                Error($"File \"{fileArgument}\" not provided or does not exist.");
                return;
            }

            logger.Log("Starting build", this.GetType().Name, LogType.INFO);
            var buildCommand = new BuildCommand(version, logger);
            buildCommand.Run(arguments);
            logger.Log("Completed Build", this.GetType().Name, LogType.INFO);

            var location = $"bin/pirate{version}";
            logger.Log($"Executing {fileName}.py", this.GetType().Name, LogType.INFO);
            var pythonEngine = new PythonEngine($"{location}/{fileName}.py");
            var result = pythonEngine.InvokeMain("main", logger); //Possibly null reference;
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