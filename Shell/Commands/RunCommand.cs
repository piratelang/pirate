using Common;
using Common.Enum;
using Interpreter;

namespace Shell.Commands
{
    public class RunCommand : Command
    {
        public ObjectSerializer ObjectSerializer { get; set; }
        public string Location { get; set; }
        public RunCommand(string Version, ILogger Logger, string location) : base(Version, Logger)
        {
            ObjectSerializer = new(location, this.Logger);
            Location = location;
        }
        public override void Run(string[] arguments)
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
            var buildCommand = new BuildCommand(Version, Logger, ObjectSerializer, Location);
            var scopeList = buildCommand.Run(arguments);
            if (scopeList == null)
            {
                Logger.Log($"Build Command returned no scopes", this.GetType().Name, LogType.ERROR);
                
            }
            Logger.Log("Completed Build", this.GetType().Name, LogType.INFO);

            var location = $"bin/pirate{Version}";
            Logger.Log($"Executing {fileName}.pirate\n", this.GetType().Name, LogType.INFO);
            
            var interpreter = new Interpreter.Interpreter(fileName, ObjectSerializer, Logger);
            var interpreterResult = interpreter.StartInterpreter();
            foreach (var item in interpreterResult)
            {
                Console.WriteLine(item.Value);
            }
        }

        public override void Help()
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
    }
}